using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EmployeeTraining.api;
using HarmonyLib;
using Lean.Pool;
using MyBox;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Collections;
using UnityEngine.UIElements.StyleSheets;

namespace EmployeeTraining.EmployeeRestocker
{
    public class RestockerLogic
    {
        private static readonly BoxSize[] BoxTowerOrder = new BoxSize[]{
            BoxSize._40x26x26, BoxSize._30x20x20, BoxSize._20x20x20, BoxSize._20x20x10, BoxSize._15x15x15, BoxSize._22x22x8, BoxSize._20x10x7, BoxSize._8x8x8
        };
        private static readonly Dictionary<BoxSize, int> BoxHeights = new Dictionary<BoxSize, int>{
            [BoxSize._20x10x7] = 223,
            [BoxSize._8x8x8] = 256,
            [BoxSize._22x22x8] = 256,
            [BoxSize._20x20x10] = 316,
            [BoxSize._15x15x15] = 462,
            [BoxSize._20x20x20] = 618,
            [BoxSize._30x20x20] = 618,
            [BoxSize._40x26x26] = 839
        };

        private RestockerState State { get => fldState.Value; set => fldState.Value = value; }
        private readonly PrivateFld<RestockerState> fldState = new PrivateFld<RestockerState>(typeof(Restocker), "m_State");
        private int TargetProductID { get => fldTargetProductID.Value; set => fldTargetProductID.Value = value; }
        private readonly PrivateFld<int> fldTargetProductID = new PrivateFld<int>(typeof(Restocker), "m_TargetProductID");
        private DisplaySlot TargetDisplaySlot { get => fldTargetDisplaySlot.Value; set => fldTargetDisplaySlot.Value = value; }
        private readonly PrivateFld<DisplaySlot> fldTargetDisplaySlot = new PrivateFld<DisplaySlot>(typeof(Restocker), "m_TargetDisplaySlot");
        private RackSlot TargetRackSlot { get => fldTargetRackSlot.Value; set => fldTargetRackSlot.Value = value; }
        private readonly PrivateFld<RackSlot> fldTargetRackSlot = new PrivateFld<RackSlot>(typeof(Restocker), "m_TargetRackSlot");
        private bool CheckTasks { get => fldCheckTasks.Value; set => fldCheckTasks.Value = value; }
        private readonly PrivateFld<bool> fldCheckTasks = new PrivateFld<bool>(typeof(Restocker), "m_CheckTasks");
        private LayerMask CurrentBoxLayer { get => fldCurrentBoxLayer.Value; set => fldCurrentBoxLayer.Value = value; }
        private readonly PrivateFld<LayerMask> fldCurrentBoxLayer = new PrivateFld<LayerMask>(typeof(Restocker), "m_CurrentBoxLayer");
        private Box Box { get => fldBox.Value; set => fldBox.Value = value; }
        private readonly PrivateFld<Box> fldBox = new PrivateFld<Box>(typeof(Restocker), "m_Box");
        private Transform BoxHolder { get => fldBoxHolder.Value; set => fldBoxHolder.Value = value; }
        private readonly PrivateFld<Transform> fldBoxHolder = new PrivateFld<Transform>(typeof(Restocker), "m_BoxHolder");
        private NavMeshAgent Agent { get => fldAgent.Value; set => fldAgent.Value = value; }
        private readonly PrivateFld<NavMeshAgent> fldAgent = new PrivateFld<NavMeshAgent>(typeof(Restocker), "m_Agent");
        private float MinFillRateForDisplaySlotsToRestock { get => fldMinFillRateForDisplaySlotsToRestock.Value; set => fldMinFillRateForDisplaySlotsToRestock.Value = value; }
        private readonly PrivateFld<float> fldMinFillRateForDisplaySlotsToRestock = new PrivateFld<float>(typeof(Restocker), "m_MinFillRateForDisplaySlotsToRestock");
        private bool IsCarryBoxToRack { get => fldIsCarryBoxToRack.Value; set => fldIsCarryBoxToRack.Value = value; }
        private readonly PrivateFld<bool> fldIsCarryBoxToRack = new PrivateFld<bool>(typeof(Restocker), "m_IsCarryBoxToRack");
        private Box TargetBox { get => fldTargetBox.Value; set => fldTargetBox.Value = value; }
        private readonly PrivateFld<Box> fldTargetBox = new PrivateFld<Box>(typeof(Restocker), "m_TargetBox");
        // m_CurrentBoostLevel
        private int CurrentBoostLevel { get => fldCurrentBoostLevel.Value; set => fldCurrentBoostLevel.Value = value; }
        private readonly PrivateFld<int> fldCurrentBoostLevel = new PrivateFld<int>(typeof(Restocker), "m_CurrentBoostLevel");
        // m_RestockerWalkingSpeeds
        private List<float> RestockerWalkingSpeeds { get => fldRestockerWalkingSpeeds.Value; set => fldRestockerWalkingSpeeds.Value = value; }
        private readonly PrivateFld<List<float>> fldRestockerWalkingSpeeds = new PrivateFld<List<float>>(typeof(Restocker), "m_RestockerWalkingSpeeds");
        // m_RestockerPlacingSpeeds
        private List<float> RestockerPlacingSpeeds { get => fldRestockerPlacingSpeeds.Value; set => fldRestockerPlacingSpeeds.Value = value; }
        private readonly PrivateFld<List<float>> fldRestockerPlacingSpeeds = new PrivateFld<List<float>>(typeof(Restocker), "m_RestockerPlacingSpeeds");


        private Dictionary<int, List<RackSlot>> RackSlots { get => fldRackSlots.Value; set => fldRackSlots.Value = value; }
        private readonly PrivateFld<Dictionary<int, List<RackSlot>>> fldRackSlots = new PrivateFld<Dictionary<int, List<RackSlot>>>(typeof(RackManager), "m_RackSlots");
        private List<Rack> Racks { get => fldRacks.Value; set => fldRacks.Value = value; }
        private readonly PrivateFld<List<Rack>> fldRacks = new PrivateFld<List<Rack>>(typeof(RackManager), "m_Racks");
 

        private readonly Action ResetTargets;
        private readonly PrivateMtd mtdResetTargets = new PrivateMtd(typeof(Restocker), "ResetTargets");
        private readonly Func<IEnumerator> TryRestocking;
        private readonly PrivateMtd<IEnumerator> mtdTryRestocking = new PrivateMtd<IEnumerator>(typeof(Restocker), "TryRestocking");
        private readonly Func<IEnumerator> PlaceBoxFromStreet;
        private readonly PrivateMtd<IEnumerator> mtdPlaceBoxFromStreet = new PrivateMtd<IEnumerator>(typeof(Restocker), "PlaceBoxFromStreet");
        private readonly Action PlaceBox;
        private readonly PrivateMtd mtdPlaceBox = new PrivateMtd(typeof(Restocker), "PlaceBox");
        private readonly Func<IEnumerator> DropBox;
        private readonly PrivateMtd<IEnumerator> mtdDropBox = new PrivateMtd<IEnumerator>(typeof(Restocker), "DropBox");
        private readonly Func<bool, IEnumerator> PickUpBox;
        private readonly PrivateMtd<IEnumerator> mtdPickUpBox = new PrivateMtd<IEnumerator>(typeof(Restocker), "PickUpBox", typeof(bool));
        private readonly Func<IEnumerator> PerformRestocking;
        private readonly PrivateMtd<IEnumerator> mtdPerformRestocking = new PrivateMtd<IEnumerator>(typeof(Restocker), "PerformRestocking");
        private readonly Func<IEnumerator> PlaceProducts;
        private readonly PrivateMtd<IEnumerator> mtdPlaceProducts = new PrivateMtd<IEnumerator>(typeof(Restocker), "PlaceProducts");
        private readonly Func<IEnumerator> PlaceBoxToRack;
        private readonly PrivateMtd<IEnumerator> mtdPlaceBoxToRack = new PrivateMtd<IEnumerator>(typeof(Restocker), "PlaceBoxToRack");
        private readonly Func<bool> GetAvailableDisplaySlotToRestock;
        private readonly PrivateMtd<bool> mtdGetAvailableDisplaySlotToRestock = new PrivateMtd<bool>(typeof(Restocker), "GetAvailableDisplaySlotToRestock");
        private readonly Func<bool> CheckForAvailableRackSlotToTakeBox;
        private readonly PrivateMtd<bool> mtdCheckForAvailableRackSlotToTakeBox = new PrivateMtd<bool>(typeof(Restocker), "CheckForAvailableRackSlotToTakeBox");
        private readonly Func<bool> CheckForAvailableRackSlotToPlaceBox;
        private readonly PrivateMtd<bool> mtdCheckForAvailableRackSlotToPlaceBox = new PrivateMtd<bool>(typeof(Restocker), "CheckForAvailableRackSlotToPlaceBox");
        // Avoid to call the vanilla side of IsDisplaySlotAvailableToRestock because it's not needed in this logic and Pallets Display can cause an error
        // private readonly Func<DisplaySlot, bool> IsDisplaySlotAvailableToRestock;
        // private readonly PrivateMtd<bool> mtdIsDisplaySlotAvailableToRestock = new PrivateMtd<bool>(typeof(Restocker), "IsDisplaySlotAvailableToRestock", typeof(DisplaySlot));
        private readonly Func<RackSlot, int, bool> IsRackSlotStillAvailable;
        private readonly PrivateMtd<bool> mtdIsRackSlotStillAvailable = new PrivateMtd<bool>(typeof(Restocker), "IsRackSlotStillAvailable", typeof(RackSlot), typeof(int));
        private readonly Func<IEnumerator> ThrowBoxToTrashBin;
        private readonly PrivateMtd<IEnumerator> mtdThrowBoxToTrashBin = new PrivateMtd<IEnumerator>(typeof(Restocker), "ThrowBoxToTrashBin");
        private readonly Func<Vector3, Quaternion, IEnumerator> GoTo;
        private readonly PrivateMtd<IEnumerator> mtdGoTo = new PrivateMtd<IEnumerator>(typeof(Restocker), "GoTo", typeof(Vector3), typeof(Quaternion));
        private readonly Func<RestockerState, IEnumerator> GoToWaiting;
        private readonly PrivateMtd<IEnumerator> mtdGoToWaiting = new PrivateMtd<IEnumerator>(typeof(Restocker), "GoToWaiting", typeof(RestockerState));
        private readonly Func<Box, RackSlot> HasBoxAtRackForMerge;
        private readonly PrivateMtd<RackSlot> mtdHasBoxAtRackForMerge = new PrivateMtd<RackSlot>(typeof(Restocker), "HasBoxAtRackForMerge", typeof(Box));
        private readonly Func<RackSlot, IEnumerator> MergeBox;
        private readonly PrivateMtd<IEnumerator> mtdMergeBox = new PrivateMtd<IEnumerator>(typeof(Restocker), "MergeBox", typeof(RackSlot));

        private readonly RestockerSkill skill;
        private readonly Restocker restocker;
        private readonly Inventory inventory = new Inventory();
        private readonly Dictionary<int, int> planList = new Dictionary<int, int>();
        private readonly Dictionary<int, int> carryingBoxes = new Dictionary<int, int>();
        private int productsNeeded;
        private int totalCarryingWeight;
        private int totalCarryingHeight;
        private readonly List<DisplaySlot> occupiedDisplaySlots = new List<DisplaySlot>();
        
        private int CarryingCapacity => skill.CarryingCapacity;
        private int CarryingMaxHeight => skill.CarryingMaxHeight;
        private float ProductPlacingInterval => skill.ProductPlacingIntv; // 0.2s
        private float UnpackingTime => skill.UnpackingTime; // 0.7s
        private float TakingBoxTime => skill.TakingBoxTime; // 0.3s
        private float ThrowingBoxTime => skill.ThrowingBoxTime;
        private float MovingSpeed => skill.AgentSpeed;
        private float AngularSpeed => skill.AgentAngularSpeed;
        private float Acceleration => skill.AgentAcceleration;
        private float TurningSpeed => skill.TurningSpeed;
        private float RotationTime => skill.RotationTime;

        // private float lastTimeTryRestocking;
        // private int streakCounter;

        public RestockerLogic(RestockerSkill skill, Restocker restocker)
        {
            this.skill = skill;
            this.restocker = restocker;

            fldState.Instance = restocker;
            fldTargetProductID.Instance = restocker;
            fldTargetDisplaySlot.Instance = restocker;
            fldTargetRackSlot.Instance = restocker;
            fldCheckTasks.Instance = restocker;
            fldCurrentBoxLayer.Instance = restocker;
            fldBox.Instance = restocker;
            fldBoxHolder.Instance = restocker;
            fldAgent.Instance = restocker;
            fldIsCarryBoxToRack.Instance = restocker;
            fldMinFillRateForDisplaySlotsToRestock.Instance = restocker;
            fldTargetBox.Instance = restocker;
            fldCurrentBoostLevel.Instance = restocker;
            fldRestockerWalkingSpeeds.Instance = restocker;
            fldRestockerPlacingSpeeds.Instance = restocker;

            fldRackSlots.Instance = Singleton<RackManager>.Instance;
            fldRacks.Instance = Singleton<RackManager>.Instance;

            ResetTargets = () => mtdResetTargets.Invoke();
            mtdResetTargets.Instance = restocker;
            TryRestocking = () => mtdTryRestocking.Invoke();
            mtdTryRestocking.Instance = restocker;
            PlaceBoxFromStreet = () => mtdPlaceBoxFromStreet.Invoke();
            mtdPlaceBoxFromStreet.Instance = restocker;
            PlaceBox = () => mtdPlaceBox.Invoke();
            mtdPlaceBox.Instance = restocker;
            DropBox = () => mtdDropBox.Invoke();
            mtdDropBox.Instance = restocker;
            PickUpBox = (isFromRack) => mtdPickUpBox.Invoke(isFromRack);
            mtdPickUpBox.Instance = restocker;
            PerformRestocking = () => mtdPerformRestocking.Invoke();
            mtdPerformRestocking.Instance = restocker;
            PlaceProducts = () => mtdPlaceProducts.Invoke();
            mtdPlaceProducts.Instance = restocker;
            PlaceBoxToRack = () => mtdPlaceBoxToRack.Invoke();
            mtdPlaceBoxToRack.Instance = restocker;
            GetAvailableDisplaySlotToRestock = () => mtdGetAvailableDisplaySlotToRestock.Invoke();
            mtdGetAvailableDisplaySlotToRestock.Instance = restocker;
            CheckForAvailableRackSlotToTakeBox = () => mtdCheckForAvailableRackSlotToTakeBox.Invoke();
            mtdCheckForAvailableRackSlotToTakeBox.Instance = restocker;
            CheckForAvailableRackSlotToPlaceBox = () => mtdCheckForAvailableRackSlotToPlaceBox.Invoke();
            mtdCheckForAvailableRackSlotToPlaceBox.Instance = restocker;
            // this.IsDisplaySlotAvailableToRestock = (displaySlot) => mtdIsDisplaySlotAvailableToRestock.Invoke(displaySlot);
            // this.mtdIsDisplaySlotAvailableToRestock.Instance = restocker;
            IsRackSlotStillAvailable = (rackSlot, productId) => mtdIsRackSlotStillAvailable.Invoke(rackSlot, productId);
            mtdIsRackSlotStillAvailable.Instance = restocker;
            ThrowBoxToTrashBin = () => mtdThrowBoxToTrashBin.Invoke();
            mtdThrowBoxToTrashBin.Instance = restocker;
            GoTo = (position, rotation) => mtdGoTo.Invoke(position, rotation);
            mtdGoTo.Instance = restocker;
            GoToWaiting = (state) => mtdGoToWaiting.Invoke(state);
            mtdGoToWaiting.Instance = restocker;
            HasBoxAtRackForMerge = (box) => mtdHasBoxAtRackForMerge.Invoke(box);
            mtdHasBoxAtRackForMerge.Instance = restocker;
            MergeBox = (slot) => mtdMergeBox.Invoke(slot);
            mtdMergeBox.Instance = restocker;
        }
        public void AfterResetRestocker()
        {
            inventory.Clear();
            carryingBoxes.Clear();
            planList.Clear();
        }

        public void Internal_DropTheBox()
        {
            // this.LogStat("Called DropTheBox");
            // Singleton<EmployeeManager>.Instance.OccupyProductID(this.TargetProductID, false);
            if (Box == null || !restocker.CarryingBox)
            {
                return;
            }
            foreach (Box box in inventory.Boxes)
            {
                Singleton<InventoryManager>.Instance.RemoveBox(box.Data);
                LeanPool.Despawn(box.gameObject, 0f);
                box.gameObject.layer = CurrentBoxLayer;
                box.ResetBox();
            }
            inventory.Clear();
            carryingBoxes.Clear();
            planList.Clear();
            Box = null;
            TargetBox = null;
            restocker.CarryingBox = false;
            State = RestockerState.IDLE;
            CheckTasks = true;
        }

        public void Internal_DropBoxToGround()
        {
            foreach (Box box in inventory.Boxes)
            {
                box.DropBox();
                box.gameObject.layer = CurrentBoxLayer;
        		box.SetOccupy(false, null);
            }
            inventory.Clear();
            carryingBoxes.Clear();
            planList.Clear();
            Box = null;
            TargetBox = null;
            restocker.CarryingBox = false;
            CheckTasks = true;
        }

        public IEnumerator Internal_TryRestocking()
        {
            if (State != RestockerState.IDLE)
            {
                yield break;
            }
            // var timestamp = Time.time;
            // if (timestamp - lastTimeTryRestocking < 10f)
            // {
            //     if (streakCounter > 10)
            //     {
            //         throw new Exception("*** Loop detected ***");
            //     }
            //     streakCounter++;
            // }
            // lastTimeTryRestocking = timestamp;

            State = RestockerState.RESTOCKING;
            ResetTargets();
            planList.Clear();
            carryingBoxes.Clear();
            MinFillRateForDisplaySlotsToRestock = 1;
            totalCarryingWeight = 0;
            totalCarryingHeight = 0;
            bool doneRestocking = false;

            List<int> productsInInventory = FindProductNeededToStock();

            restocker.FreeTargetDisplaySlot();
            yield return null;

            for (int j = 0; j < productsInInventory.Count; j++)
            {
                TargetProductID = productsInInventory[j];
                productsNeeded = GetTotalDisplayCapacity(TargetProductID) - Singleton<InventoryManager>.Instance.Products[TargetProductID];
                // bool pickedUp = false;
                LogStat($"TargetProductID={TargetProductID}, Demand={productsNeeded}");
                List<DisplaySlot> checkedDisplaySlot = new List<DisplaySlot>();
                while (productsNeeded > 0 && GetAvailableDisplaySlotToRestock() && restocker.ManagementData.RestockShelf)
                {
                    CheckTasks = false;
    				bool isBoxFromRack = true;
                    if (restocker.ManagementData.PickUpBoxGround)
                    {
                        TargetBox = GetBoxFromStreet();
                    }
                    if (TargetBox != null && restocker.ManagementData.PickUpBoxGround)
                    {
                        TargetBox.SetOccupy(true, restocker.transform);
                        isBoxFromRack = false;
                        Vector3 target = Vector3.MoveTowards(TargetBox.transform.position, restocker.transform.position, 0.35f);
						Vector3 position = TargetBox.transform.position;
						position.y = restocker.transform.position.y;
                        Quaternion rotation = Quaternion.LookRotation(position, Vector3.up);
                        yield return restocker.StartCoroutine(GoTo(target, rotation));
                    }
                    else
                    {
                        TargetBox = null;
                        bool foundAvailableRack = false;
                        while (!foundAvailableRack && CheckForAvailableRackSlotToTakeBox())
                        {
                            LogStat($"going to the rack {TargetRackSlot}");
                            yield return restocker.StartCoroutine(GoTo(TargetRackSlot.InteractionPosition, TargetRackSlot.InteractionRotation));
                            if (TargetRackSlot != null)
                            {
                                bool isRackActive = TargetRackSlot.gameObject.activeInHierarchy;
                                bool isRackStillAvailable = IsRackSlotStillAvailable(TargetRackSlot, TargetProductID);
                                Plugin.LogDebug($"active={isRackActive}, still available={isRackStillAvailable}");
                                if (isRackActive && isRackStillAvailable)
                                {
                                    foundAvailableRack = true;
                                }
                            }
                        }

                        if (!foundAvailableRack)
                        {
                            break;
                        }
                    }

                    if (!IsDisplaySlotAvailableToRestock(TargetDisplaySlot))
                    {
                        TargetDisplaySlot.OccupiedRestocker = null;
                        occupiedDisplaySlots.Remove(TargetDisplaySlot);
                        if (!GetAvailableDisplaySlotToRestock())
                        {
                            break;
                        }
                    }
                    if (TargetBox == null || !TargetBox.IsBoxOccupied || TargetBox.OccupyOwner == restocker.transform)
                    {
                        yield return restocker.StartCoroutine(PickUpBox(isBoxFromRack));
                        if (isBoxFromRack && TargetRackSlot != null && restocker.ManagementData.RemoveLabelRack && !TargetRackSlot.HasBox)
                        {
                            TargetRackSlot.ClearLabel();
                        }
                    }
                }
            }

            LogStat("has finished collection, will restock");
            var productIds = inventory.ProductIds;
            foreach (int id in productIds)
            {
                TargetProductID = id;
                //this.GetAvailableDisplaySlotToRestock();

                // Get every available slot to restock
                List<DisplaySlot> slots = Singleton<DisplayManager>.Instance.GetDisplaySlots(TargetProductID, false);
    			slots.AddRange(Singleton<DisplayManager>.Instance.GetLabeledEmptyDisplaySlots(TargetProductID));

                Box = inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == TargetProductID);
                bool foundAvailableDisplaySlot = false;
                foreach (DisplaySlot slot in slots)
                {
                    TargetDisplaySlot = slot;
                    if (!IsDisplaySlotAvailableToRestock(slot))
                    {
                        continue;
                    }
                    if (TargetBox != null && !TargetBox.HasProducts
                        || TargetBox != null && TargetBox.OccupyOwner != restocker.transform)
                    {
                        break;
                    }
                    LogStat($"going to the display {TargetDisplaySlot}");
                    TargetDisplaySlot.OccupiedRestocker = restocker;
                    occupiedDisplaySlots.Add(TargetDisplaySlot);
                    yield return restocker.StartCoroutine(
                            GoTo(TargetDisplaySlot.InteractionPosition - TargetDisplaySlot.InteractionPositionForward * 0.3f,
                                TargetDisplaySlot.InteractionRotation));
                    if (TargetDisplaySlot != null
                            && TargetDisplaySlot.gameObject.activeInHierarchy
                            && !TargetDisplaySlot.Full
                            && IsDisplaySlotAvailableToRestock(TargetDisplaySlot) 
                            && TargetProductID == TargetDisplaySlot.ProductID)
                    {
                        foundAvailableDisplaySlot = true;
                        break;
                    }
                    TargetDisplaySlot.OccupiedRestocker = null;
                    occupiedDisplaySlots.Remove(TargetDisplaySlot);
                }

                if (!foundAvailableDisplaySlot)
                {
                    TargetDisplaySlot.OccupiedRestocker = null;
                    occupiedDisplaySlots.Remove(TargetDisplaySlot);
                }
                else
                {
                    LogStat($"restocking to {TargetDisplaySlot}");
                    yield return restocker.StartCoroutine(PerformRestocking());
                    LogStat($"done restocking");
                }
                doneRestocking = true;
            }

            LogStat($"goes dropping box");
            yield return restocker.StartCoroutine(DropBox());
            if (doneRestocking)
            {
                skill.AddExp(2);
                yield break;
            }

            if (!HasBox())
            {
                restocker.FreeTargetDisplaySlot();
                planList.Clear();
                IsCarryBoxToRack = false;
                if (restocker.ManagementData.PickUpBoxGround)
                {
                    yield return restocker.StartCoroutine(PlaceBoxFromStreet());
                }
                if (!restocker.CarryingBox && !IsCarryBoxToRack && State != RestockerState.IDLE)
                {
                    yield return restocker.StartCoroutine(restocker.SoftResetRestocker());
                }
            }
        }

        private Box GetBoxFromStreet()
        {
            // Organize boxes in StorageStreet to trace GetBoxFromStreet()
            Singleton<StorageStreet>.Instance.boxes.RemoveAll(x => x.Racked);
            Singleton<StorageStreet>.Instance.boxes.RemoveAll(x => !x.gameObject.activeInHierarchy);
            // Get a target box from the street
            List<Box> boxesOnStreet = Singleton<StorageStreet>.Instance.GetBoxesFromStreet();
            bool includeEmptyBox = restocker.ManagementData.PickUpBoxGround;
            List<Box> boxes = boxesOnStreet.FindAll(x => x.HasProducts && !x.Racked && x.Product.ID == TargetProductID
                && x.gameObject.activeInHierarchy
                && (!x.IsBoxOccupied || x.IsBoxOccupied && x.OccupyOwner == restocker.transform));
            if (includeEmptyBox)
            {
                boxes.AddRange(boxesOnStreet.FindAll(x => !x.HasProducts
                    && (!x.IsBoxOccupied || x.IsBoxOccupied && x.OccupyOwner == restocker.transform)));
            }
            
            return boxes.Count > 0 ? boxes.GetRandom() : null;
        }

        public List<int> FindProductNeededToStock()
        {
            var customers = Singleton<ShoppingCustomerList>.Instance.CustomersInShopping;
            bool isCustomerInShopping = customers.Count > 0 && customers.Any(c => c.ShoppingList != null && c.ShoppingList.HasProduct);
            bool canFullyStock = !isCustomerInShopping;
            // Plugin.LogDebug($"Restocker[{skill.Id}] canFullyStock: {canFullyStock}");
            // Plugin.LogDebug($"ActiveCustomers={customers.Count}, {customers.Select(c => $"[{c.ShoppingList?.Products.Keys.Join()}]").Join()}");

            var carrying = CollectProductsCarrying();
            var capacities = (from i in Singleton<InventoryManager>.Instance.Products
                    select new KeyValuePair<int, int>(i.Key, GetTotalDisplayCapacity(i.Key)))
                    .ToDictionary(p => p.Key, p => p.Value);
            var demand = (from i in Singleton<InventoryManager>.Instance.Products
                    where i.Value + carrying.Get(i.Key, 0) < capacities[i.Key] * (canFullyStock ? 1 : 0.8f)
                    orderby i.Value + carrying.Get(i.Key, 0) ascending
                    select new KeyValuePair<int, int>(i.Key, capacities[i.Key] - i.Value - carrying.Get(i.Key, 0)))
                    .ToList();
            // Plugin.LogDebug($"Restocker[{this.restocker.RestockerID}] Demands:");
            // Plugin.LogDebug(demand.Select(p => $"[productID={Singleton<IDManager>.Instance.ProductSO(p.Key).name}, count={p.Value}]").Join(delimiter: ", "));

            // Plugin.LogDebug($"Products in a box:");
            // Singleton<IDManager>.Instance.Products.ForEach(p => Plugin.LogDebug($"{p.ID}\t{p.GridLayoutInBox.productCount}"));

            int totalWeightOfPlan = 0;
            int totalHeightOfPlan = 0;
            foreach (KeyValuePair<int, int> p in demand)
            {
                if (totalWeightOfPlan > CarryingCapacity || totalHeightOfPlan > CarryingMaxHeight) break;

                int id = p.Key;
                int count = p.Value;
                List<List<BoxData>> racks = GetBoxesInRacks(id);
                if (racks != null)
                {
                    BoxSize boxSize = Singleton<IDManager>.Instance.ProductSO(id).GridLayoutInBox.boxSize;
                    var boxHeight = BoxHeights[boxSize];
                    foreach (List<BoxData> rack in racks)
                    {
                        Stack<BoxData> boxes = new Stack<BoxData>(rack);
                        while (boxes.Count > 0)
                        {
                            BoxData box = boxes.Pop();
                            int boxWeight = ProductWeight.CalcWeight(box);
                            if (count > 0
                                    && (totalWeightOfPlan == 0 || totalWeightOfPlan + boxWeight <= CarryingCapacity)
                                    && (totalHeightOfPlan == 0 || totalHeightOfPlan + boxHeight <= CarryingMaxHeight))
                            {
                                if (!planList.TryAdd(id, box.ProductCount))
                                {
                                    planList[id] += box.ProductCount;
                                }
                                count -= box.ProductCount;
                                totalWeightOfPlan += boxWeight;
                                totalHeightOfPlan += boxHeight;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            // Plugin.LogDebug($"Restocker[{this.skill.Id}] Planned:");
            // Plugin.LogDebug(this.planList.Select(p => $"[productID={Singleton<IDManager>.Instance.ProductSO(p.Key).name}, count={p.Value}]").Join(delimiter: ", "));
            // Plugin.LogDebug($"Weight: {totalWeightOfPlan}, Capacity: {this.CarryingCapacity}");

            return planList.Select(p => p.Key).ToList();
        }

        private int GetTotalDisplayCapacity(int productId)
        {
            List<DisplaySlot> displaySlots = Singleton<DisplayManager>.Instance.GetDisplaySlots(productId, false)?
                    .Distinct().ToList();
            var total = displaySlots?.Sum(i => GetCapacityInDisplaySlot(i)) ?? 0;
            return total;
        }

        private int GetCapacityInDisplaySlot(DisplaySlot displaySlot)
        {
            if (displaySlot.Data == null || displaySlot.Data.FirstItemID <= 0)
            {
                return 0;
            }
            foreach (IModdedDisplayHandler handler in ModdedDisplayManager.Registry)
            {
                if (handler.IsTargetDisplay(displaySlot))
                {
                    int capacity = handler.GetProductCountOfGridLayout(displaySlot);
                    // Plugin.LogDebug($"{displaySlot.Display.name}: {displaySlot.Data.FirstItemCount}/{capacity}");
                    return capacity;
                }
            }
            ProductSO productSO = Singleton<IDManager>.Instance.ProductSO(displaySlot.Data.FirstItemID);
            return productSO.GridLayoutInStorage.productCount;
        }

        private Dictionary<int, int> CollectProductsCarrying()
        {
            return (from k in from i in RestockerSkillManager.Instance.GetActiveLogics()
                from j in i.planList select j
                    group k.Value by k.Key)
                .ToDictionary(g => g.Key, g => g.Sum());
        }

        private List<List<BoxData>> GetBoxesInRacks(int productID)
        {
            IEnumerable<List<BoxData>> boxesInRacks;
            IEnumerable<BoxData> boxesOnStreet = from box in Singleton<StorageStreet>.Instance.boxes
                    where box.HasProducts && !box.Racked && box.Product.ID == productID && box.gameObject.activeInHierarchy
                    select box.Data;

            if (RackSlots.ContainsKey(productID))
            {
                boxesInRacks = from rack in RackSlots[productID]
                        where rack.HasProduct
                        select rack.Data.RackedBoxDatas;
            }
            else
            {
                boxesInRacks = new List<List<BoxData>>().AsEnumerable();
            }
            
            return boxesInRacks.Prepend(boxesOnStreet.ToList()).ToList();
        }

        private List<Box> GetBoxListOnStreet()
        {
            List<int> idList = restocker.GetAvailableProductIDList();
            List<Box> boxes = Singleton<StorageStreet>.Instance.boxes;
            boxes.RemoveAll(x => x.Racked);
            boxes.RemoveAll(x => !x.gameObject.activeInHierarchy);
            List<Box> list = boxes.FindAll(x => x.HasProducts && !x.Racked && idList.Contains(x.Data.ProductID) && x.gameObject.activeInHierarchy);
            list.AddRange(boxes.FindAll(x => !x.HasProducts));
            // Plugin.LogDebug($"BoxListOnStreet: Count={list.Count()} {list.ToBoxStackInfo()}");
            return list;
        }

        private int GetRackCapacityOfSpaceFor(int productID)
        {
            LogStat($"called GetRackCapacityOfSpaceFor: productId={productID}");
            Plugin.LogDebug($"Racks: {Racks?.Count}");
            ProductSO pso = Singleton<IDManager>.Instance.ProductSO(productID);
            BoxSO bso = Singleton<IDManager>.Instance.Boxes.First(so => so.BoxSize == pso.GridLayoutInBox.boxSize);
            int slots = Racks.SelectMany(rack => rack.RackSlots)
                    .Where(slot => slot.Data.ProductID == productID && !slot.Full)
                    .Sum(slot => bso.GridLayout.boxCount - slot.Data.BoxCount);
            if (slots > 0)
            {
                Plugin.LogDebug($"Slots: {slots}");
                return slots;
            }
            LogStat($"collecting empty rack slots");
            return Racks.SelectMany(rack => rack.RackSlots)
                    .Where(slot => slot.Data.ProductID == -1 && !slot.HasBox)
                    .Sum(slot => bso.GridLayout.boxCount);
        }

        private Dictionary<int, int> CollectBoxesCarryingFromStreet()
        {
            return (from k in from i in RestockerSkillManager.Instance.GetActiveLogics()
                from j in i.carryingBoxes select j
                    group k.Value by k.Key)
                .ToDictionary(g => g.Key, g => g.Sum());
        }

        public IEnumerator Internal_PlaceBoxFromStreet()
        {
            LogStat($"called PlaceBoxFromStreet");
            IsCarryBoxToRack = false;
            List<Box> boxesToCarry = new List<Box>();
            int totalCarryingWeight = 0;
            int totalCarryingHeight = 0;
            var totalCarryingBoxes = CollectBoxesCarryingFromStreet();
            foreach (Box box in GetBoxListOnStreet())
            {
                if (box == null || box.IsBoxOccupied && box.OccupyOwner != restocker.transform || box.Racked || !box.HasProducts)
                {
                    continue;
                }
                int boxWeight = ProductWeight.CalcWeight(box.Data);
                int boxHeight = BoxHeights[box.Data.Size];
                if (totalCarryingWeight > 0 && boxWeight + totalCarryingWeight > CarryingCapacity
                    || totalCarryingHeight > 0 && boxHeight + totalCarryingHeight > CarryingMaxHeight)
                {
                    continue;
                }
                int pid = box.Data.ProductID;
                int productBoxCnt = boxesToCarry.Where(b => b.Data.ProductID == pid).Count();
                if (productBoxCnt < GetRackCapacityOfSpaceFor(pid) - totalCarryingBoxes.Get(pid, 0))
                {
                    box.SetOccupy(true, restocker.transform);
                    boxesToCarry.Add(box);
                    totalCarryingWeight += boxWeight;
                    totalCarryingHeight += boxHeight;
                    if (!carryingBoxes.TryAdd(pid, 1))
                    {
                        carryingBoxes[pid] += 1;
                    }
                }
            }

            foreach (Box box in boxesToCarry)
            {
                TargetBox = box;
                TargetProductID = TargetBox.Data.ProductID;
                Box = TargetBox;
                Vector3 target = Vector3.MoveTowards(TargetBox.transform.position, restocker.transform.position, 0.35f);
                Quaternion rotation = Quaternion.LookRotation(TargetBox.transform.position, Vector3.up);
                yield return restocker.StartCoroutine(GoTo(target, rotation));
                if (TargetBox == null || TargetBox.Racked)
                {
                    DoneCarryingBox(TargetBox);
                    TargetBox = null;
                    TargetProductID = -1;
                }
                else
                {
                    LogStat($"picking up from street: {TargetBox.ToBoxInfo()}");
                    yield return restocker.StartCoroutine(PickUpBox(false));
                    if (HasBox())
                    {
                        IsCarryBoxToRack = true;
                    }
                }
            }

            yield return restocker.StartCoroutine(DropBox());
            if (IsCarryBoxToRack)
            {
                skill.AddExp(2);
            }

            LogStat($"finished PlaceBoxFromStreet");
        }

        public IEnumerator Internal_DropBox()
        {
            // this.LogStat($"Called DropBox");
            if (TargetDisplaySlot != null && TargetDisplaySlot.IsOccupiedByOthers(restocker))
            {
                occupiedDisplaySlots.Remove(TargetDisplaySlot);
                TargetDisplaySlot.OccupiedRestocker = null;
            }
            if (!HasBox())
            {
                yield break;
            }
            if (inventory.Boxes.Any(b => !b.HasProducts))
            {
                yield return restocker.StartCoroutine(ThrowBoxToTrashBin());
            }
            if (inventory.Boxes.Any(b => b.HasProducts))
            {
                yield return restocker.StartCoroutine(PlaceBoxToRack());
            }
        }

        public IEnumerator Internal_PerformRestocking()
        {
            DoneRestocking(Box);
            yield return restocker.StartCoroutine(PlaceProducts());
            Box = inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == TargetProductID);
            while (Box != null && Box.HasProducts && GetAvailableDisplaySlotToRestock())
            {
                yield return restocker.StartCoroutine(GoTo(
                        TargetDisplaySlot.InteractionPosition - TargetDisplaySlot.InteractionPositionForward * 0.3f,
                        TargetDisplaySlot.InteractionRotation));
                DoneRestocking(Box);
                if (TargetDisplaySlot == null
                        || !TargetDisplaySlot.gameObject.activeInHierarchy
                        || TargetDisplaySlot.Full
                        || !IsDisplaySlotAvailableToRestock(TargetDisplaySlot)
                        || TargetProductID != TargetDisplaySlot.ProductID)
                {
                    occupiedDisplaySlots.Remove(TargetDisplaySlot);
                    TargetDisplaySlot.OccupiedRestocker = null;
                }
                else
                {
                    LogStat("calling PlaceProducts");
                    yield return restocker.StartCoroutine(PlaceProducts());
                }
                Box = inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == TargetProductID);
            }
            if (State == RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT)
            {
                restocker.StartCoroutine(DropBox());
            }
        }

        public IEnumerator Internal_PlaceBoxToRack()
        {
            Box = inventory.Boxes.FirstOrDefault(b => b.HasProducts);
            LogStat($"called PlaceBoxToRack");
            TargetProductID = Box.Data.ProductID;

            while (restocker.CarryingBox)
            {
                RackSlot rackSlot = HasBoxAtRackForMerge(Box);
                LogStat($"trying to merge {Box.ToBoxInfo()} to {rackSlot}");
                yield return restocker.StartCoroutine(MergeBox(rackSlot));
                if (!Box.HasProducts)
                {
                    DoneCarryingBox(TargetProductID);
                    Box = inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == TargetProductID);
                    if (Box != null)
                    {
                        continue;
                    }
                    Box = inventory.Boxes.FirstOrDefault(b => b.HasProducts);
                    if (Box == null)
                    {
                        Box = inventory.Boxes.FirstOrDefault();
                    }
                    else
                    {
                        TargetProductID = Box.Data.ProductID;
                        continue;
                    }
                }

                if (Box.HasProducts)
                {
                    if (!CheckForAvailableRackSlotToPlaceBox())
                    {
                        break;
                    }

                    LogStat($"going to rack {TargetRackSlot}");
                    yield return restocker.StartCoroutine(
                            GoTo(TargetRackSlot.InteractionPosition, TargetRackSlot.InteractionRotation));
                    
                    if (!(TargetRackSlot == null) 
                            && TargetRackSlot.gameObject.activeInHierarchy 
                            && !TargetRackSlot.Full 
                            && (!TargetRackSlot.HasProduct
                                || TargetRackSlot.Data.ProductID == TargetProductID)
                            && (TargetRackSlot.Data.ProductID == -1
                                || TargetRackSlot.Data.ProductID == TargetProductID)
                            && (TargetRackSlot.HasProduct || !TargetRackSlot.HasBox))
                    {
                        LogStat($"placing box {Box.ToBoxInfo()}");
                        DoneCarryingBox(Box);
                        PlaceBox();
                    }
                }
                if (!HasBox() || !inventory.Boxes.Any(b => b.HasProducts))
                {
                    if (inventory.Boxes.Any(b => !b.HasProducts))
                    {
                        yield return restocker.StartCoroutine(ThrowBoxToTrashBin());
                    }

                    LogStat($"done placing box");
                    restocker.CarryingBox = false;
                    State = RestockerState.IDLE;
                    restocker.StartCoroutine(TryRestocking());
                    yield break;
                }
            }

            if (restocker.CarryingBox)
            {
                LogStat($"not done placing box, wating for rack to place");
                restocker.StartCoroutine(GoToWaiting(RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT));
            }
        }

        public IEnumerator Internal_ThrowBoxToTrashBin()
        {
            LogStat($"Called ThrowBoxToTrashBin");
            yield return restocker.StartCoroutine(GoTo(
                Singleton<FurnitureManager>.Instance.TrashBin.position,
                Singleton<FurnitureManager>.Instance.TrashBin.rotation));

            Box = inventory.Boxes.Where(b => !b.HasProducts).FirstOrDefault();
            
            while (Box != null)
            {
                yield return new WaitForSeconds(ThrowingBoxTime);
                // this.LogStat();
                Singleton<InventoryManager>.Instance.RemoveBox(Box.Data);
                LeanPool.Despawn(Box.gameObject, 0f);
                Box.gameObject.layer = CurrentBoxLayer;
                Box.ResetBox();
                inventory.Remove(Box);
                ArrangeBoxTower();
                DoneCarryingBox(Box);
                Box = inventory.Boxes.Where(b => !b.HasProducts).FirstOrDefault();
            }
            LogStat("threw boxes to trash bin");

            Box = inventory.Boxes.FirstOrDefault();
            // this.LogStat();
            if (Box == null)
            {
                TargetBox = null;
                restocker.CarryingBox = false;
                State = RestockerState.IDLE;
                // Singleton<EmployeeManager>.Instance.OccupyProductID(this.TargetProductID, false);
                restocker.StartCoroutine(TryRestocking());
            }
            else if (State == RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT)
            {
                Plugin.LogDebug($"Restocker[{skill.Id}] waiting for avalilable rack slot");
                restocker.StartCoroutine(PlaceBoxToRack());
            }
        }

        public IEnumerator Internal_MoveTo(Vector3 target)
        {
            var boost = RestockerWalkingSpeeds[CurrentBoostLevel] / 2f;
            var speed = MovingSpeed * boost;
            var linearMotion = speed >= 10;
            Agent.speed = speed;
            Agent.angularSpeed = AngularSpeed * boost;
            Agent.acceleration = Acceleration * boost;
            // Plugin.LogDebug($"Agent: speed={this.Agent.speed}, angularSpeed={this.Agent.angularSpeed}, acceleration={this.Agent.acceleration}");

            if (NavMesh.SamplePosition(target, out NavMeshHit navMeshHit, 20f, -1))
            {
                Agent.SetDestination(navMeshHit.position);
            }
            else
            {
                Agent.SetDestination(target);
            }
            while (Vector3.Distance(restocker.transform.position, Agent.destination) > Agent.stoppingDistance)
            {
                if (linearMotion)
                {
                    Agent.velocity = (Agent.steeringTarget - restocker.transform.position).normalized * Agent.speed;
                    restocker.transform.forward = Agent.steeringTarget - restocker.transform.position;
                }
                else
                {
                    if (Agent.velocity.magnitude > 0f)
                    {
                        restocker.transform.rotation = Quaternion.Slerp(restocker.transform.rotation, Quaternion.LookRotation(Agent.velocity), TurningSpeed * boost * Time.deltaTime);
                    }
                }
                yield return null;
            }
        }

        public IEnumerator Internal_RotateTo(Quaternion rotation)
        {
            restocker.transform.DORotateQuaternion(rotation, RotationTime);
            yield return new WaitForSeconds(RotationTime);
            yield break;
        }

        public IEnumerator Internal_PickUpBox(bool isFromRack)
        {
            if (isFromRack)
            {
                while (productsNeeded > 0)
                {
                    if (TargetRackSlot.Data.BoxCount == 0 || TargetRackSlot.Data.BoxID == -1) yield break;
                    BoxData nextBoxData = TargetRackSlot.Data.RackedBoxDatas.Last();
                    int boxWeight = ProductWeight.CalcWeight(nextBoxData);
                    int boxHeight = BoxHeights[nextBoxData.Size];
                    if (totalCarryingWeight > 0 && boxWeight + totalCarryingWeight > CarryingCapacity
                        || totalCarryingHeight > 0 && boxHeight + totalCarryingHeight > CarryingMaxHeight)
                    {
                        productsNeeded -= nextBoxData.ProductCount;
                        DoneRestocking(nextBoxData);
                        continue;
                    }
                    Box box = TargetRackSlot.TakeBoxFromRack();
                    if (box == null)
                    {
                        yield break;
                    }
                    if (!carryingBoxes.TryAdd(box.Data.ProductID, 1))
                    {
                        carryingBoxes[box.Data.ProductID] += 1;
                    }
                    LogStat($"picking up {box.ToBoxInfo()} from a rack");
                    yield return GrabBox(box, isFromRack, boxWeight, boxHeight);
                }
            }
            else
            {
                // From street
                int boxWeight = ProductWeight.CalcWeight(TargetBox);
                int boxHeight = BoxHeights[TargetBox.Size];
                if (totalCarryingWeight > 0 && boxWeight + totalCarryingWeight > CarryingCapacity
                    || totalCarryingHeight > 0 && boxHeight + totalCarryingHeight > CarryingMaxHeight)
                {
                    TargetBox.SetOccupy(false, null);
                    yield break;
                }

                Box box = TargetBox;
                box.FrezeeBox();
                if (TargetBox != null && TargetBox.OccupyOwner != restocker.transform)
                {
                    Box = null;
                    TargetBox = null;
                    yield break;
                }
                LogStat($"picking up {box.ToBoxInfo()} from streets or floor");
                yield return GrabBox(box, isFromRack, boxWeight, boxHeight);
            }
        }

        private IEnumerator GrabBox(Box box, bool isFromRack, int boxWeight, int boxHeight)
        {
            productsNeeded -= box.Data.ProductCount;

            Collider[] componentsInChildren = box.GetComponentsInChildren<Collider>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].isTrigger = true;
            }
            if (!isFromRack)
            {
                Singleton<StorageStreet>.Instance.OnTakeBoxFromStreet?.Invoke(box);
            }
            inventory.Add(box);
            box.FrezeeBox();
            box.transform.SetParent(BoxHolder);
            box.transform.DOLocalRotate(Vector3.zero, 0.3f, RotateMode.Fast);
            ArrangeBoxTower();
            Box = box;
            Box.SetOccupy(true, restocker.transform);
            Box.Racked = false;
            totalCarryingWeight += boxWeight;
            totalCarryingHeight += boxHeight;
            CurrentBoxLayer = box.gameObject.layer;
            box.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            restocker.CarryingBox = true;
            yield return new WaitForSeconds(TakingBoxTime);
        }

        private void DoneRestocking(Box box)
        {
            DoneRestocking(box.Data);
        }

        private void DoneRestocking(BoxData box)
        {
            int id = box.ProductID;
            if (planList.ContainsKey(id))
            {
                int fullCount = GetCapacityInDisplaySlot(TargetDisplaySlot);
                int countToRestock = Math.Min(box.ProductCount, fullCount - TargetDisplaySlot.Data.FirstItemCount);
                if (planList[id] <= countToRestock)
                {
                    planList.Remove(id);
                }
                else
                {
                    planList[id] -= countToRestock;
                }
            }
        }

        private void DoneCarryingBox(Box box)
        {
            if (box == null || box.Data.ProductID == -1) return;
            int id = box.Data.ProductID;
            DoneCarryingBox(id);
        }
        private void DoneCarryingBox(int id)
        {
            if (id == -1) return;
            if (carryingBoxes.ContainsKey(id))
            {
                if (carryingBoxes[id] <= 1)
                {
                    carryingBoxes.Remove(id);
                }
                else
                {
                    carryingBoxes[id] -= 1;
                }
            }
        }

        public IEnumerator Internal_PlaceProducts()
        {
            if (Box == null || TargetProductID != TargetDisplaySlot.ProductID)
            {
                yield break;
            }
            float boost = RestockerPlacingSpeeds[CurrentBoostLevel] / 0.2f;
            if (!Box.IsOpen)
            {
                Box.OpenBox();
                // Plugin.LogDebug($"UnpackingTime: {UnpackingTime}");
                yield return new WaitForSeconds(UnpackingTime * boost);
            }
            if (TargetProductID != TargetDisplaySlot.ProductID)
            {
                yield break;
            }
            int exp = 0;
            while (TargetDisplaySlot != null && !TargetDisplaySlot.Full && Box.HasProducts)
            {
                Product productFromBox = null;
                try
                {
                    productFromBox = Box.GetProductFromBox();
                }
                catch (ArgumentOutOfRangeException) { } // It can happen accidentally...
                if (productFromBox == null)
                {
                    break;
                }

                TargetDisplaySlot.AddProduct(TargetProductID, productFromBox);
                Singleton<InventoryManager>.Instance.AddProductToDisplay(new ItemQuantity{
                    Products = new Dictionary<int, int>{
                        { TargetProductID, 1 }
                    }
                });
                exp++;
                yield return new WaitForSeconds(ProductPlacingInterval * boost);
            }
            occupiedDisplaySlots.Remove(TargetDisplaySlot);
            TargetDisplaySlot.OccupiedRestocker = null;
            skill.AddExp(exp);
            yield break;
        }

        public void Internal_PlaceBox()
        {
            LogStat($"Called PlaceBox");
            if (Box == null)
            {
                return;
            }
            if (TargetRackSlot.IsBoxAlreadyExistInRack(Box.BoxID, Box))
            {
                return;
            }
            if (Box.Racked)
            {
                return;
            }
            Plugin.LogDebug($"Restocker[{skill.Id}] placing {Box?.ToBoxInfo()} on {TargetDisplaySlot}");
            Box.gameObject.layer = CurrentBoxLayer;
            Collider[] componentsInChildren = Box.GetComponentsInChildren<Collider>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].isTrigger = false;
            }
            TargetRackSlot.AddBox(Box.BoxID, Box);
            Box.Racked = true;
            Box.SetOccupy(false, null);
            TargetBox = null;

            inventory.Remove(Box);
            ArrangeBoxTower();

            var nextBox = inventory.Boxes.Where(b => b.Data.ProductID == TargetProductID).FirstOrDefault();
            if (nextBox == null)
            {
                nextBox = inventory.Boxes.FirstOrDefault(b => b.HasProducts);
            }
            if (nextBox != null)
            {
                TargetProductID = nextBox.Data.ProductID;
            }
            Box = nextBox;
        }

        public bool Internal_GetAvailableDisplaySlotToRestock()
        {
            Plugin.LogDebug($"Restocker[{skill.Id}] called GetAvailableDisplaySlotToRestock");
            List<DisplaySlot> displaySlots = Singleton<DisplayManager>.Instance.GetDisplaySlots(TargetProductID, false);
            if (displaySlots == null || displaySlots.Count <= 0)
            {
                Plugin.LogDebug($"-> Not found");
                return false;
            }
            DisplaySlot displaySlot = displaySlots.FirstOrDefault(d => IsDisplaySlotAvailableToRestock(d));
            if (displaySlot == null)
            {
                Plugin.LogDebug($"-> finding labeledEmptyDisplaySlots");
                List<DisplaySlot> labeledEmptyDisplaySlots = Singleton<DisplayManager>.Instance.GetLabeledEmptyDisplaySlots(TargetProductID);
                if (labeledEmptyDisplaySlots == null || labeledEmptyDisplaySlots.Count <= 0)
                {
                    Plugin.LogDebug($"-> Not found");
                    return false;
                }
                DisplaySlot targetDisplaySlot = labeledEmptyDisplaySlots[UnityEngine.Random.Range(0, labeledEmptyDisplaySlots.Count)];
                TargetDisplaySlot = targetDisplaySlot;
            }
            else
            {
                Plugin.LogDebug($"-> Found: {displaySlot}");
                TargetDisplaySlot = displaySlot;
            }
            if (!occupiedDisplaySlots.Contains(TargetDisplaySlot))
            {
                occupiedDisplaySlots.Add(TargetDisplaySlot);
            }
            TargetDisplaySlot.OccupiedRestocker = restocker;
            return true;
        }

        public bool IsDisplaySlotAvailableToRestock(DisplaySlot displaySlot)
        {
            if (displaySlot.Data == null || displaySlot.Data.FirstItemID <= 0
                    || displaySlot.IsOccupiedByOthers(restocker))
            {
                return false;
            }
            ProductSO productSO = Singleton<IDManager>.Instance.ProductSO(displaySlot.Data.FirstItemID);
            if (Box != null && Box.Data.ProductID == productSO.ID)
            {
                return !displaySlot.Full;
            }
            return true;
        }

        private void ArrangeBoxTower()
        {
            var tower = new List<Box>(inventory.Boxes);
            tower.Sort((a, b) => BoxTowerOrder.IndexOfItem(a.Size) - BoxTowerOrder.IndexOfItem(b.Size));
            var height = 0f;
            foreach(Box box in tower)
            {
                box.transform.DOLocalMove(new Vector3(0, height, 0), 0.3f, false);
                height += box.GetComponent<BoxCollider>().size.y;
            }
        }

        private bool HasBox()
        {
            return inventory.Count > 0;
        }

        public void LogStat(string msg = null)
        {
            var call = msg != null ? $"{msg} " : ""; 
            Plugin.LogDebug($"Restocker[{skill.Id}] {call}carryingBox={Box}, boxCount={inventory.Count}");
            Plugin.LogDebug(inventory.Boxes.ToBoxStackInfo());
        }

        internal void AfterFreeTargetDisplaySlot()
        {
            occupiedDisplaySlots
                    .Where(s => s.IsOccupiedByMe(restocker))
                    .ForEach(s => s.OccupiedRestocker = null);
            occupiedDisplaySlots.Clear();
        }

        public class Inventory : List<InventorySlot>
        {
            public IEnumerable<Box> Boxes => this.Select(s => s.Box);

            public void Add(Box box)
            {
                Add(new InventorySlot(box));
            }

            public void Remove(Box box)
            {
                Remove(Find(s => s.Box.Equals(box)));
            }

            public IEnumerable<int> ProductIds => this.Where(s => s.Box.HasProducts).Select(s => s.Box.Data.ProductID).Distinct();

        }

        public class InventorySlot
        {
            public Box Box;

            public InventorySlot(Box box)
            {
                Box = box;
            }
        }

    }
}