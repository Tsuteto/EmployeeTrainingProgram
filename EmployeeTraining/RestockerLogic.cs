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

namespace EmployeeTraining
{
    public class RestockerLogic
    {
        private static readonly BoxSize[] BoxTowerOrder = new BoxSize[]{
            BoxSize._40x26x26, BoxSize._30x20x20, BoxSize._20x20x20, BoxSize._20x20x10, BoxSize._15x15x15, BoxSize._20x10x7, BoxSize._8x8x8
        };
        private static readonly Dictionary<BoxSize, int> BoxHeights = new Dictionary<BoxSize, int>{
            [BoxSize._20x10x7] = 223,
            [BoxSize._8x8x8] = 256,
            [BoxSize._20x20x10] = 316,
            [BoxSize._15x15x15] = 462,
            [BoxSize._20x20x20] = 618,
            [BoxSize._30x20x20] = 618,
            [BoxSize._40x26x26] = 839
        };

        private RestockerState State { get => this.fldState.Value; set => this.fldState.Value = value; }
        private readonly PrivateFld<RestockerState> fldState = new PrivateFld<RestockerState>(typeof(Restocker), "m_State");
        private int TargetProductID { get => this.fldTargetProductID.Value; set => this.fldTargetProductID.Value = value; }
        private readonly PrivateFld<int> fldTargetProductID = new PrivateFld<int>(typeof(Restocker), "m_TargetProductID");
        private DisplaySlot TargetDisplaySlot { get => this.fldTargetDisplaySlot.Value; set => this.fldTargetDisplaySlot.Value = value; }
        private readonly PrivateFld<DisplaySlot> fldTargetDisplaySlot = new PrivateFld<DisplaySlot>(typeof(Restocker), "m_TargetDisplaySlot");
        private RackSlot TargetRackSlot { get => this.fldTargetRackSlot.Value; set => this.fldTargetRackSlot.Value = value; }
        private readonly PrivateFld<RackSlot> fldTargetRackSlot = new PrivateFld<RackSlot>(typeof(Restocker), "m_TargetRackSlot");
        private bool CheckTasks { get => this.fldCheckTasks.Value; set => this.fldCheckTasks.Value = value; }
        private readonly PrivateFld<bool> fldCheckTasks = new PrivateFld<bool>(typeof(Restocker), "m_CheckTasks");
        private LayerMask CurrentBoxLayer { get => this.fldCurrentBoxLayer.Value; set => this.fldCurrentBoxLayer.Value = value; }
        private readonly PrivateFld<LayerMask> fldCurrentBoxLayer = new PrivateFld<LayerMask>(typeof(Restocker), "m_CurrentBoxLayer");
        private Box Box { get => this.fldBox.Value; set => this.fldBox.Value = value; }
        private readonly PrivateFld<Box> fldBox = new PrivateFld<Box>(typeof(Restocker), "m_Box");
        private Transform BoxHolder { get => this.fldBoxHolder.Value; set => this.fldBoxHolder.Value = value; }
        private readonly PrivateFld<Transform> fldBoxHolder = new PrivateFld<Transform>(typeof(Restocker), "m_BoxHolder");
        private NavMeshAgent Agent { get => this.fldAgent.Value; set => this.fldAgent.Value = value; }
        private readonly PrivateFld<NavMeshAgent> fldAgent = new PrivateFld<NavMeshAgent>(typeof(Restocker), "m_Agent");
        private float MinFillRateForDisplaySlotsToRestock { get => this.fldMinFillRateForDisplaySlotsToRestock.Value; set => this.fldMinFillRateForDisplaySlotsToRestock.Value = value; }
        private readonly PrivateFld<float> fldMinFillRateForDisplaySlotsToRestock = new PrivateFld<float>(typeof(Restocker), "m_MinFillRateForDisplaySlotsToRestock");
        private bool IsCarryBoxToRack { get => this.fldIsCarryBoxToRack.Value; set => this.fldIsCarryBoxToRack.Value = value; }
        private readonly PrivateFld<bool> fldIsCarryBoxToRack = new PrivateFld<bool>(typeof(Restocker), "m_IsCarryBoxToRack");
        private Box TargetBox { get => this.fldTargetBox.Value; set => this.fldTargetBox.Value = value; }
        private readonly PrivateFld<Box> fldTargetBox = new PrivateFld<Box>(typeof(Restocker), "m_TargetBox");
        // m_CurrentBoostLevel
        private int CurrentBoostLevel { get => this.fldCurrentBoostLevel.Value; set => this.fldCurrentBoostLevel.Value = value; }
        private readonly PrivateFld<int> fldCurrentBoostLevel = new PrivateFld<int>(typeof(Restocker), "m_CurrentBoostLevel");
        // m_RestockerWalkingSpeeds
        private List<float> RestockerWalkingSpeeds { get => this.fldRestockerWalkingSpeeds.Value; set => this.fldRestockerWalkingSpeeds.Value = value; }
        private readonly PrivateFld<List<float>> fldRestockerWalkingSpeeds = new PrivateFld<List<float>>(typeof(Restocker), "m_RestockerWalkingSpeeds");
        // m_RestockerPlacingSpeeds
        private List<float> RestockerPlacingSpeeds { get => this.fldRestockerPlacingSpeeds.Value; set => this.fldRestockerPlacingSpeeds.Value = value; }
        private readonly PrivateFld<List<float>> fldRestockerPlacingSpeeds = new PrivateFld<List<float>>(typeof(Restocker), "m_RestockerPlacingSpeeds");


        private Dictionary<int, List<RackSlot>> RackSlots { get => this.fldRackSlots.Value; set => this.fldRackSlots.Value = value; }
        private readonly PrivateFld<Dictionary<int, List<RackSlot>>> fldRackSlots = new PrivateFld<Dictionary<int, List<RackSlot>>>(typeof(RackManager), "m_RackSlots");
        private List<Rack> Racks { get => this.fldRacks.Value; set => this.fldRacks.Value = value; }
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
        
        private int CarryingCapacity => this.skill.CarryingCapacity;
        private int CarryingMaxHeight => this.skill.CarryingMaxHeight;
        private float ProductPlacingInterval => this.skill.ProductPlacingIntv; // 0.2s
        private float UnpackingTime => this.skill.UnpackingTime; // 0.7s
        private float TakingBoxTime => this.skill.TakingBoxTime; // 0.3s
        private float ThrowingBoxTime => this.skill.ThrowingBoxTime;
        private float MovingSpeed => this.skill.AgentSpeed;
        private float AngularSpeed => this.skill.AgentAngularSpeed;
        private float Acceleration => this.skill.AgentAcceleration;
        private float TurningSpeed => this.skill.TurningSpeed;
        private float RotationTime => this.skill.RotationTime;

        // private float lastTimeTryRestocking;
        // private int streakCounter;

        public RestockerLogic(RestockerSkill skill, Restocker restocker)
        {
            this.skill = skill;
            this.restocker = restocker;

            this.fldState.Instance = restocker;
            this.fldTargetProductID.Instance = restocker;
            this.fldTargetDisplaySlot.Instance = restocker;
            this.fldTargetRackSlot.Instance = restocker;
            this.fldCheckTasks.Instance = restocker;
            this.fldCurrentBoxLayer.Instance = restocker;
            this.fldBox.Instance = restocker;
            this.fldBoxHolder.Instance = restocker;
            this.fldAgent.Instance = restocker;
            this.fldIsCarryBoxToRack.Instance = restocker;
            this.fldMinFillRateForDisplaySlotsToRestock.Instance = restocker;
            this.fldTargetBox.Instance = restocker;
            this.fldCurrentBoostLevel.Instance = restocker;
            this.fldRestockerWalkingSpeeds.Instance = restocker;
            this.fldRestockerPlacingSpeeds.Instance = restocker;

            this.fldRackSlots.Instance = Singleton<RackManager>.Instance;
            this.fldRacks.Instance = Singleton<RackManager>.Instance;

            this.ResetTargets = () => mtdResetTargets.Invoke();
            this.mtdResetTargets.Instance = restocker;
            this.TryRestocking = () => mtdTryRestocking.Invoke();
            this.mtdTryRestocking.Instance = restocker;
            this.PlaceBoxFromStreet = () => mtdPlaceBoxFromStreet.Invoke();
            this.mtdPlaceBoxFromStreet.Instance = restocker;
            this.PlaceBox = () => mtdPlaceBox.Invoke();
            this.mtdPlaceBox.Instance = restocker;
            this.DropBox = () => mtdDropBox.Invoke();
            this.mtdDropBox.Instance = restocker;
            this.PickUpBox = (isFromRack) => mtdPickUpBox.Invoke(isFromRack);
            this.mtdPickUpBox.Instance = restocker;
            this.PerformRestocking = () => mtdPerformRestocking.Invoke();
            this.mtdPerformRestocking.Instance = restocker;
            this.PlaceProducts = () => mtdPlaceProducts.Invoke();
            this.mtdPlaceProducts.Instance = restocker;
            this.PlaceBoxToRack = () => mtdPlaceBoxToRack.Invoke();
            this.mtdPlaceBoxToRack.Instance = restocker;
            this.GetAvailableDisplaySlotToRestock = () => mtdGetAvailableDisplaySlotToRestock.Invoke();
            this.mtdGetAvailableDisplaySlotToRestock.Instance = restocker;
            this.CheckForAvailableRackSlotToTakeBox = () => mtdCheckForAvailableRackSlotToTakeBox.Invoke();
            this.mtdCheckForAvailableRackSlotToTakeBox.Instance = restocker;
            this.CheckForAvailableRackSlotToPlaceBox = () => mtdCheckForAvailableRackSlotToPlaceBox.Invoke();
            this.mtdCheckForAvailableRackSlotToPlaceBox.Instance = restocker;
            // this.IsDisplaySlotAvailableToRestock = (displaySlot) => mtdIsDisplaySlotAvailableToRestock.Invoke(displaySlot);
            // this.mtdIsDisplaySlotAvailableToRestock.Instance = restocker;
            this.IsRackSlotStillAvailable = (rackSlot, productId) => mtdIsRackSlotStillAvailable.Invoke(rackSlot, productId);
            this.mtdIsRackSlotStillAvailable.Instance = restocker;
            this.ThrowBoxToTrashBin = () => mtdThrowBoxToTrashBin.Invoke();
            this.mtdThrowBoxToTrashBin.Instance = restocker;
            this.GoTo = (position, rotation) => mtdGoTo.Invoke(position, rotation);
            this.mtdGoTo.Instance = restocker;
            this.GoToWaiting = (state) => mtdGoToWaiting.Invoke(state);
            this.mtdGoToWaiting.Instance = restocker;
            this.HasBoxAtRackForMerge = (box) => mtdHasBoxAtRackForMerge.Invoke(box);
            this.mtdHasBoxAtRackForMerge.Instance = restocker;
            this.MergeBox = (slot) => mtdMergeBox.Invoke(slot);
            this.mtdMergeBox.Instance = restocker;
        }
        public void AfterResetRestocker()
        {
            this.inventory.Clear();
            this.carryingBoxes.Clear();
            this.planList.Clear();
        }

        public void Internal_DropTheBox()
        {
            // this.LogStat("Called DropTheBox");
            // Singleton<EmployeeManager>.Instance.OccupyProductID(this.TargetProductID, false);
            if (this.Box == null || !restocker.CarryingBox)
            {
                return;
            }
            foreach (Box box in this.inventory.Boxes)
            {
                Singleton<InventoryManager>.Instance.RemoveBox(box.Data);
                LeanPool.Despawn(box.gameObject, 0f);
                box.gameObject.layer = this.CurrentBoxLayer;
                box.ResetBox();
            }
            this.inventory.Clear();
            this.carryingBoxes.Clear();
            this.planList.Clear();
            this.Box = null;
    		this.TargetBox = null;
            restocker.CarryingBox = false;
            this.State = RestockerState.IDLE;
            this.CheckTasks = true;
        }

        public void Internal_DropBoxToGround()
        {
            foreach (Box box in this.inventory.Boxes)
            {
                box.DropBox();
                box.gameObject.layer = this.CurrentBoxLayer;
        		box.SetOccupy(false, null);
            }
            this.inventory.Clear();
            this.carryingBoxes.Clear();
            this.planList.Clear();
            this.Box = null;
    		this.TargetBox = null;
            restocker.CarryingBox = false;
            this.CheckTasks = true;
        }

        public IEnumerator Internal_TryRestocking()
        {
            if (this.State != RestockerState.IDLE)
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

            this.State = RestockerState.RESTOCKING;
            this.ResetTargets();
            this.planList.Clear();
            this.carryingBoxes.Clear();
            this.MinFillRateForDisplaySlotsToRestock = 1;
            this.totalCarryingWeight = 0;
            this.totalCarryingHeight = 0;
            bool doneRestocking = false;

            List<int> productsInInventory = this.FindProductNeededToStock();

            restocker.FreeTargetDisplaySlot();
            yield return null;

            for (int j = 0; j < productsInInventory.Count; j++)
            {
                this.TargetProductID = productsInInventory[j];
                this.productsNeeded = this.GetTotalDisplayCapacity(this.TargetProductID) - Singleton<InventoryManager>.Instance.Products[this.TargetProductID];
                // bool pickedUp = false;
                this.LogStat($"TargetProductID={this.TargetProductID}, Demand={this.productsNeeded}");
                List<DisplaySlot> checkedDisplaySlot = new List<DisplaySlot>();
                while (this.productsNeeded > 0 && this.GetAvailableDisplaySlotToRestock())
                {
                    this.CheckTasks = false;
    				bool isBoxFromRack = true;
                    this.TargetBox = this.GetBoxFromStreet();
                    if (this.TargetBox != null)
                    {
                        this.TargetBox.SetOccupy(true, restocker.transform);
                        isBoxFromRack = false;
                        Vector3 target = Vector3.MoveTowards(this.TargetBox.transform.position, restocker.transform.position, 0.35f);
						Vector3 position = this.TargetBox.transform.position;
						position.y = restocker.transform.position.y;
                        Quaternion rotation = Quaternion.LookRotation(position, Vector3.up);
                        yield return restocker.StartCoroutine(this.GoTo(target, rotation));
                    }
                    else
                    {
                        bool foundAvailableRack = false;
                        while (!foundAvailableRack && this.CheckForAvailableRackSlotToTakeBox())
                        {
                            this.LogStat($"going to the rack {this.TargetRackSlot}");
                            yield return restocker.StartCoroutine(this.GoTo(this.TargetRackSlot.InteractionPosition, this.TargetRackSlot.InteractionRotation));
                            if (this.TargetRackSlot != null)
                            {
                                bool isRackActive = this.TargetRackSlot.gameObject.activeInHierarchy;
                                bool isRackStillAvailable = this.IsRackSlotStillAvailable(this.TargetRackSlot, this.TargetProductID);
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

                    if (!this.IsDisplaySlotAvailableToRestock(this.TargetDisplaySlot))
                    {
                        this.TargetDisplaySlot.OccupiedRestocker = null;
                        this.occupiedDisplaySlots.Remove(TargetDisplaySlot);
                        if (!this.GetAvailableDisplaySlotToRestock())
                        {
                            break;
                        }
                    }
                    if (this.TargetBox == null || !this.TargetBox.IsBoxOccupied || this.TargetBox.OccupyOwner == restocker.transform)
                    {
                        yield return restocker.StartCoroutine(this.PickUpBox(isBoxFromRack));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            this.LogStat("completed collection, going to restock");
            var productIds = this.inventory.ProductIds;
            foreach (int id in productIds)
            {
                this.TargetProductID = id;
                //this.GetAvailableDisplaySlotToRestock();

                // Get every available slot to restock
                List<DisplaySlot> slots = Singleton<DisplayManager>.Instance.GetDisplaySlots(this.TargetProductID, false);
    			slots.AddRange(Singleton<DisplayManager>.Instance.GetLabeledEmptyDisplaySlots(this.TargetProductID));

                this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
                bool foundAvailableDisplaySlot = false;
                foreach (DisplaySlot slot in slots)
                {
                    this.TargetDisplaySlot = slot;
                    if (!this.IsDisplaySlotAvailableToRestock(slot))
                    {
                        continue;
                    }
                    if ((this.TargetBox != null && !this.TargetBox.HasProducts)
                        || (this.TargetBox != null && this.TargetBox.OccupyOwner != restocker.transform))
                    {
                        break;
                    }
                    this.LogStat($"going to the display {this.TargetDisplaySlot}");
            		this.TargetDisplaySlot.OccupiedRestocker = restocker;
                    this.occupiedDisplaySlots.Add(TargetDisplaySlot);
                    yield return restocker.StartCoroutine(
                            this.GoTo(this.TargetDisplaySlot.InteractionPosition - this.TargetDisplaySlot.InteractionPositionForward * 0.3f,
                                this.TargetDisplaySlot.InteractionRotation));
                    if (this.TargetDisplaySlot != null
                            && this.TargetDisplaySlot.gameObject.activeInHierarchy
                            && !this.TargetDisplaySlot.Full
                            && this.IsDisplaySlotAvailableToRestock(this.TargetDisplaySlot) 
                            && this.TargetProductID == this.TargetDisplaySlot.ProductID)
                    {
                        foundAvailableDisplaySlot = true;
                        break;
                    }
                    this.TargetDisplaySlot.OccupiedRestocker = null;
                    this.occupiedDisplaySlots.Remove(TargetDisplaySlot);
                }

                if (!foundAvailableDisplaySlot)
                {
                    this.TargetDisplaySlot.OccupiedRestocker = null;
                    this.occupiedDisplaySlots.Remove(TargetDisplaySlot);
                }
                else
                {
                    this.LogStat($"restocking to {this.TargetDisplaySlot}");
                    yield return restocker.StartCoroutine(this.PerformRestocking());
                    this.LogStat($"done restocking");
                }
                doneRestocking = true;
            }

            this.LogStat($"goes dropping box");
            yield return restocker.StartCoroutine(this.DropBox());
            if (doneRestocking)
            {
                skill.AddExp(2);
                yield break;
            }

            if (!this.HasBox())
            {
                restocker.FreeTargetDisplaySlot();
                this.planList.Clear();
                this.restocker.StartCoroutine(this.PlaceBoxFromStreet());
            }
        }

        private Box GetBoxFromStreet()
        {
            // Organize boxes in StorageStreet to trace GetBoxFromStreet()
            Singleton<StorageStreet>.Instance.boxes.RemoveAll(x => x.Racked);
            Singleton<StorageStreet>.Instance.boxes.RemoveAll(x => !x.gameObject.activeInHierarchy);
            // Get a target box from the street
            List<Box> boxesOnStreet = Singleton<StorageStreet>.Instance.GetBoxesFromStreet();
            List<Box> boxes = boxesOnStreet.FindAll(x => x.HasProducts && !x.Racked && x.Product.ID == this.TargetProductID
                && x.gameObject.activeInHierarchy
                && (!x.IsBoxOccupied || x.IsBoxOccupied && x.OccupyOwner == this.restocker.transform));
            boxes.AddRange(boxesOnStreet.FindAll(x => !x.HasProducts));
            
            return boxes.Count > 0 ? boxes.GetRandom<Box>() : null;
        }

        public List<int> FindProductNeededToStock()
        {
            var customers = Singleton<ShoppingCustomerList>.Instance.CustomersInShopping;
            bool isCustomerInShopping = customers.Count > 0 && customers.Any(c => c.ShoppingList != null && c.ShoppingList.HasProduct);
            bool canFullyStock = !isCustomerInShopping;
            // Plugin.LogDebug($"Restocker[{skill.Id}] canFullyStock: {canFullyStock}");
            // Plugin.LogDebug($"ActiveCustomers={customers.Count}, {customers.Select(c => $"[{c.ShoppingList?.Products.Keys.Join()}]").Join()}");

            var carrying = this.CollectProductsCarrying();
            var capacities = (from i in Singleton<InventoryManager>.Instance.Products
                    select new KeyValuePair<int, int>(i.Key, this.GetTotalDisplayCapacity(i.Key)))
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
                if (totalWeightOfPlan > this.CarryingCapacity || totalHeightOfPlan > this.CarryingMaxHeight) break;

                int id = p.Key;
                int count = p.Value;
                List<List<BoxData>> racks = this.GetBoxesInRacks(id);
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
                                    && (totalWeightOfPlan == 0 || totalWeightOfPlan + boxWeight <= this.CarryingCapacity)
                                    && (totalHeightOfPlan == 0 || totalHeightOfPlan + boxHeight <= this.CarryingMaxHeight))
                            {
                                if (!this.planList.TryAdd(id, box.ProductCount))
                                {
                                    this.planList[id] += box.ProductCount;
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
            var total = displaySlots?.Sum(i => this.GetCapacityInDisplaySlot(i)) ?? 0;
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

            if (this.RackSlots.ContainsKey(productID))
            {
                boxesInRacks = from rack in this.RackSlots[productID]
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
            this.LogStat($"called GetRackCapacityOfSpaceFor: productId={productID}");
            Plugin.LogDebug($"Racks: {this.Racks?.Count}");
            ProductSO pso = Singleton<IDManager>.Instance.ProductSO(productID);
            BoxSO bso = Singleton<IDManager>.Instance.Boxes.First(so => so.BoxSize == pso.GridLayoutInBox.boxSize);
            int slots = this.Racks.SelectMany(rack => rack.RackSlots)
                    .Where(slot => slot.Data.ProductID == productID && !slot.Full)
                    .Sum(slot => bso.GridLayout.boxCount - slot.Data.BoxCount);
            if (slots > 0)
            {
                Plugin.LogDebug($"Slots: {slots}");
                return slots;
            }
            this.LogStat($"collecting empty rack slots");
            return this.Racks.SelectMany(rack => rack.RackSlots)
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
            this.LogStat($"called PlaceBoxFromStreet");
            this.IsCarryBoxToRack = false;
            List<Box> boxesToCarry = new List<Box>();
            int totalCarryingWeight = 0;
            int totalCarryingHeight = 0;
            var totalCarryingBoxes = this.CollectBoxesCarryingFromStreet();
            foreach (Box box in this.GetBoxListOnStreet())
            {
                if (box == null || box.IsBoxOccupied && box.OccupyOwner != restocker.transform || box.Racked || !box.HasProducts)
                {
                    continue;
                }
                int boxWeight = ProductWeight.CalcWeight(box.Data);
                int boxHeight = BoxHeights[box.Data.Size];
                if (totalCarryingWeight > 0 && boxWeight + totalCarryingWeight > this.CarryingCapacity
                    || totalCarryingHeight > 0 && boxHeight + totalCarryingHeight > this.CarryingMaxHeight)
                {
                    continue;
                }
                int pid = box.Data.ProductID;
                int productBoxCnt = boxesToCarry.Where(b => b.Data.ProductID == pid).Count();
                if (productBoxCnt < this.GetRackCapacityOfSpaceFor(pid) - totalCarryingBoxes.Get(pid, 0))
                {
                    box.SetOccupy(true, restocker.transform);
                    boxesToCarry.Add(box);
                    totalCarryingWeight += boxWeight;
                    totalCarryingHeight += boxHeight;
                    if (!this.carryingBoxes.TryAdd(pid, 1))
                    {
                        this.carryingBoxes[pid] += 1;
                    }
                }
            }

            foreach (Box box in boxesToCarry)
            {
                this.TargetBox = box;
                this.TargetProductID = this.TargetBox.Data.ProductID;
                this.Box = this.TargetBox;
                Vector3 target = Vector3.MoveTowards(this.TargetBox.transform.position, restocker.transform.position, 0.35f);
                Quaternion rotation = Quaternion.LookRotation(this.TargetBox.transform.position, Vector3.up);
                yield return restocker.StartCoroutine(this.GoTo(target, rotation));
                if (this.TargetBox == null || this.TargetBox.Racked)
                {
                    this.DoneCarryingBox(this.TargetBox);
                    this.TargetBox = null;
                    this.TargetProductID = -1;
                }
                else
                {
                    this.LogStat($"picking up from street: {this.TargetBox.ToBoxInfo()}");
                    yield return restocker.StartCoroutine(this.PickUpBox(false));
                    if (this.HasBox())
                    {
                        this.IsCarryBoxToRack = true;
                    }
                }
            }

            yield return restocker.StartCoroutine(this.DropBox());

            // Logic to carry empty boxes inserted in vanilla 0.3
            // but not needed here since they're already carried in the previous loop
            
            if (!this.IsCarryBoxToRack)
            {
                restocker.StartCoroutine(this.GoToWaiting(RestockerState.IDLE));
            }
            else
            {
                this.skill.AddExp(2);
            }

            this.LogStat($"finished PlaceBoxFromStreet");
        }

        public IEnumerator Internal_DropBox()
        {
            // this.LogStat($"Called DropBox");
            if (this.TargetDisplaySlot != null && this.TargetDisplaySlot.IsOccupiedByOthers(restocker))
            {
                this.occupiedDisplaySlots.Remove(TargetDisplaySlot);
                this.TargetDisplaySlot.OccupiedRestocker = null;
            }
            if (!this.HasBox())
            {
                yield break;
            }
            if (this.inventory.Boxes.Any(b => !b.HasProducts))
            {
                yield return restocker.StartCoroutine(this.ThrowBoxToTrashBin());
            }
            if (this.inventory.Boxes.Any(b => b.HasProducts))
            {
                yield return restocker.StartCoroutine(this.PlaceBoxToRack());
            }
        }

        public IEnumerator Internal_PerformRestocking()
        {
            this.DoneRestocking(this.Box);
            yield return restocker.StartCoroutine(this.PlaceProducts());
            this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
            while (this.Box != null && this.Box.HasProducts && this.GetAvailableDisplaySlotToRestock())
            {
                yield return restocker.StartCoroutine(this.GoTo(
                        this.TargetDisplaySlot.InteractionPosition - this.TargetDisplaySlot.InteractionPositionForward * 0.3f,
                        this.TargetDisplaySlot.InteractionRotation));
                this.DoneRestocking(this.Box);
                if (this.TargetDisplaySlot == null
                        || !this.TargetDisplaySlot.gameObject.activeInHierarchy
                        || this.TargetDisplaySlot.Full
                        || !this.IsDisplaySlotAvailableToRestock(this.TargetDisplaySlot)
                        || this.TargetProductID != this.TargetDisplaySlot.ProductID)
                {
                    this.occupiedDisplaySlots.Remove(TargetDisplaySlot);
                    this.TargetDisplaySlot.OccupiedRestocker = null;
                }
                else
                {
                    this.LogStat("calling PlaceProducts");
                    yield return restocker.StartCoroutine(this.PlaceProducts());
                }
                this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
            }
            if (this.State == RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT)
            {
                restocker.StartCoroutine(this.DropBox());
            }
        }

        public IEnumerator Internal_PlaceBoxToRack()
        {
            this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts);
            this.LogStat($"called PlaceBoxToRack");
            this.TargetProductID = this.Box.Data.ProductID;

            while (restocker.CarryingBox)
            {
                RackSlot rackSlot = this.HasBoxAtRackForMerge(this.Box);
                this.LogStat($"trying to merge {this.Box.ToBoxInfo()} to {rackSlot}");
                yield return this.restocker.StartCoroutine(this.MergeBox(rackSlot));
                if (!this.Box.HasProducts)
                {
                    this.DoneCarryingBox(this.TargetProductID);
                    this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
                    if (this.Box != null)
                    {
                        continue;
                    }
                    this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts);
                    if (this.Box == null)
                    {
                        this.Box = this.inventory.Boxes.FirstOrDefault();
                    }
                    else
                    {
                        this.TargetProductID = this.Box.Data.ProductID;
                        continue;
                    }
                }

                if (this.Box.HasProducts)
                {
                    if (!this.CheckForAvailableRackSlotToPlaceBox())
                    {
                        break;
                    }

                    this.LogStat($"going to rack {this.TargetRackSlot}");
                    yield return restocker.StartCoroutine(
                            this.GoTo(this.TargetRackSlot.InteractionPosition, this.TargetRackSlot.InteractionRotation));
                    
                    if (!(this.TargetRackSlot == null) 
                            && this.TargetRackSlot.gameObject.activeInHierarchy 
                            && !this.TargetRackSlot.Full 
                            && (!this.TargetRackSlot.HasProduct
                                || this.TargetRackSlot.Data.ProductID == this.TargetProductID)
                            && (this.TargetRackSlot.Data.ProductID == -1
                                || this.TargetRackSlot.Data.ProductID == this.TargetProductID)
                            && (this.TargetRackSlot.HasProduct || !this.TargetRackSlot.HasBox))
                    {
                        this.LogStat($"placing box {this.Box.ToBoxInfo()}");
                        this.DoneCarryingBox(this.Box);
                        this.PlaceBox();
                    }
                }
                if (!this.HasBox() || !this.inventory.Boxes.Any(b => b.HasProducts))
                {
                    if (this.inventory.Boxes.Any(b => !b.HasProducts))
                    {
                        yield return restocker.StartCoroutine(this.ThrowBoxToTrashBin());
                    }

                    this.LogStat($"done placing box");
                    restocker.CarryingBox = false;
                    this.State = RestockerState.IDLE;
                    restocker.StartCoroutine(this.TryRestocking());
                    yield break;
                }
            }

            if (restocker.CarryingBox)
            {
                this.LogStat($"not done placing box, wating for rack to place");
                restocker.StartCoroutine(this.GoToWaiting(RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT));
            }
        }

        public IEnumerator Internal_ThrowBoxToTrashBin()
        {
            this.LogStat($"Called ThrowBoxToTrashBin");
            yield return restocker.StartCoroutine(this.GoTo(
                Singleton<FurnitureManager>.Instance.TrashBin.position,
                Singleton<FurnitureManager>.Instance.TrashBin.rotation));

            this.Box = this.inventory.Boxes.Where(b => !b.HasProducts).FirstOrDefault();
            
            while (this.Box != null)
            {
                yield return new WaitForSeconds(this.ThrowingBoxTime);
                // this.LogStat();
                Singleton<InventoryManager>.Instance.RemoveBox(this.Box.Data);
                LeanPool.Despawn(this.Box.gameObject, 0f);
                this.Box.gameObject.layer = this.CurrentBoxLayer;
                this.Box.ResetBox();
                this.inventory.Remove(this.Box);
                this.ArrangeBoxTower();
                this.DoneCarryingBox(this.Box);
                this.Box = this.inventory.Boxes.Where(b => !b.HasProducts).FirstOrDefault();
            }
            this.LogStat("threw boxes to trash bin");

            this.Box = this.inventory.Boxes.FirstOrDefault();
            // this.LogStat();
            if (this.Box == null)
            {
        		this.TargetBox = null;
                restocker.CarryingBox = false;
                this.State = RestockerState.IDLE;
                // Singleton<EmployeeManager>.Instance.OccupyProductID(this.TargetProductID, false);
                restocker.StartCoroutine(this.TryRestocking());
            }
            else if (this.State == RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT)
            {
                Plugin.LogDebug($"Restocker[{skill.Id}] waiting for avalilable rack slot");
                restocker.StartCoroutine(this.PlaceBoxToRack());
            }
        }

        public IEnumerator Internal_MoveTo(Vector3 target)
        {
            var boost = this.RestockerWalkingSpeeds[this.CurrentBoostLevel] / 2f;
            var speed = this.MovingSpeed * boost;
            var linearMotion = speed >= 10;
            this.Agent.speed = speed;
            this.Agent.angularSpeed = this.AngularSpeed * boost;
            this.Agent.acceleration = this.Acceleration * boost;
            // Plugin.LogDebug($"Agent: speed={this.Agent.speed}, angularSpeed={this.Agent.angularSpeed}, acceleration={this.Agent.acceleration}");

            if (NavMesh.SamplePosition(target, out NavMeshHit navMeshHit, 20f, -1))
            {
                this.Agent.SetDestination(navMeshHit.position);
            }
            else
            {
                this.Agent.SetDestination(target);
            }
            while (Vector3.Distance(restocker.transform.position, this.Agent.destination) > this.Agent.stoppingDistance)
            {
                if (linearMotion)
                {
                    this.Agent.velocity = (this.Agent.steeringTarget - restocker.transform.position).normalized * this.Agent.speed;
                    restocker.transform.forward = this.Agent.steeringTarget - restocker.transform.position;
                }
                else
                {
                    if (this.Agent.velocity.magnitude > 0f)
                    {
                        restocker.transform.rotation = Quaternion.Slerp(restocker.transform.rotation, Quaternion.LookRotation(this.Agent.velocity), this.TurningSpeed * boost * Time.deltaTime);
                    }
                }
                yield return null;
            }
        }

        public IEnumerator Internal_RotateTo(Quaternion rotation)
        {
            restocker.transform.DORotateQuaternion(rotation, this.RotationTime);
            yield return new WaitForSeconds(this.RotationTime);
            yield break;
        }

        public IEnumerator Internal_PickUpBox(bool isFromRack)
        {
            if (isFromRack)
            {
                while (this.productsNeeded > 0)
                {
                    if (this.TargetRackSlot.Data.BoxCount == 0 || this.TargetRackSlot.Data.BoxID == -1) yield break;
                    BoxData nextBoxData = this.TargetRackSlot.Data.RackedBoxDatas.Last();
                    int boxWeight = ProductWeight.CalcWeight(nextBoxData);
                    int boxHeight = BoxHeights[nextBoxData.Size];
                    if (this.totalCarryingWeight > 0 && boxWeight + this.totalCarryingWeight > this.CarryingCapacity
                        || this.totalCarryingHeight > 0 && boxHeight + this.totalCarryingHeight > this.CarryingMaxHeight)
                    {
                        this.productsNeeded -= nextBoxData.ProductCount;
                        this.DoneRestocking(nextBoxData);
                        continue;
                    }
                    Box box = this.TargetRackSlot.TakeBoxFromRack();
                    if (box == null)
                    {
                        yield break;
                    }
                    if (!this.carryingBoxes.TryAdd(box.Data.ProductID, 1))
                    {
                        this.carryingBoxes[box.Data.ProductID] += 1;
                    }
                    yield return this.GrabBox(box, isFromRack, boxWeight, boxHeight);
                }
            }
            else
            {
                // From street
                int boxWeight = ProductWeight.CalcWeight(this.TargetBox);
                int boxHeight = BoxHeights[this.TargetBox.Size];
                if (this.totalCarryingWeight > 0 && boxWeight + this.totalCarryingWeight > this.CarryingCapacity
                    || this.totalCarryingHeight > 0 && boxHeight + this.totalCarryingHeight > this.CarryingMaxHeight)
                {
                    this.TargetBox.SetOccupy(false, null);
                    yield break;
                }

                Box box = this.TargetBox;
                box.FrezeeBox();
                if (this.TargetBox != null && this.TargetBox.OccupyOwner != restocker.transform)
                {
                    this.Box = null;
                    this.TargetBox = null;
                    yield break;
                }
                LogStat($"picking up {box.ToBoxInfo()} from rack");
                yield return this.GrabBox(box, isFromRack, boxWeight, boxHeight);
            }
        }

        private IEnumerator GrabBox(Box box, bool isFromRack, int boxWeight, int boxHeight)
        {
            this.productsNeeded -= box.Data.ProductCount;

            Collider[] componentsInChildren = box.GetComponentsInChildren<Collider>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].isTrigger = true;
            }
            if (!isFromRack)
            {
                Singleton<StorageStreet>.Instance.OnTakeBoxFromStreet?.Invoke(box);
            }
            this.inventory.Add(box);
            box.FrezeeBox();
            box.transform.SetParent(this.BoxHolder);
            box.transform.DOLocalRotate(Vector3.zero, 0.3f, RotateMode.Fast);
            this.ArrangeBoxTower();
            this.Box = box;
		    this.Box.SetOccupy(true, restocker.transform);
            this.Box.Racked = false;
            this.totalCarryingWeight += boxWeight;
            this.totalCarryingHeight += boxHeight;
            this.CurrentBoxLayer = box.gameObject.layer;
            box.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            restocker.CarryingBox = true;
            yield return new WaitForSeconds(this.TakingBoxTime);
        }

        private void DoneRestocking(Box box)
        {
            this.DoneRestocking(box.Data);
        }

        private void DoneRestocking(BoxData box)
        {
            int id = box.ProductID;
            if (this.planList.ContainsKey(id))
            {
                int fullCount = this.GetCapacityInDisplaySlot(this.TargetDisplaySlot);
                int countToRestock = Math.Min(box.ProductCount, fullCount - this.TargetDisplaySlot.Data.FirstItemCount);
                if (this.planList[id] <= countToRestock)
                {
                    this.planList.Remove(id);
                }
                else
                {
                    this.planList[id] -= countToRestock;
                }
            }
        }

        private void DoneCarryingBox(Box box)
        {
            if (box == null || box.Data.ProductID == -1) return;
            int id = box.Data.ProductID;
            this.DoneCarryingBox(id);
        }
        private void DoneCarryingBox(int id)
        {
            if (id == -1) return;
            if (this.carryingBoxes.ContainsKey(id))
            {
                if (this.carryingBoxes[id] <= 1)
                {
                    this.carryingBoxes.Remove(id);
                }
                else
                {
                    this.carryingBoxes[id] -= 1;
                }
            }
        }

        public IEnumerator Internal_PlaceProducts()
        {
            if (this.Box == null || this.TargetProductID != this.TargetDisplaySlot.ProductID)
            {
                yield break;
            }
            float boost = this.RestockerPlacingSpeeds[this.CurrentBoostLevel] / 0.2f;
            if (!this.Box.IsOpen)
            {
                this.Box.OpenBox();
                // Plugin.LogDebug($"UnpackingTime: {UnpackingTime}");
                yield return new WaitForSeconds(this.UnpackingTime * boost);
            }
            if (this.TargetProductID != this.TargetDisplaySlot.ProductID)
            {
                yield break;
            }
            int exp = 0;
            while (this.TargetDisplaySlot != null && !this.TargetDisplaySlot.Full && this.Box.HasProducts)
            {
                Product productFromBox = this.Box.GetProductFromBox();
                if (productFromBox == null)
                {
                    break;
                }
                this.TargetDisplaySlot.AddProduct(this.TargetProductID, productFromBox);
                Singleton<InventoryManager>.Instance.AddProductToDisplay(new ItemQuantity{
                    Products = new Dictionary<int, int>{
                        { this.TargetProductID, 1 }
                    }
                });
                exp++;
                yield return new WaitForSeconds(this.ProductPlacingInterval * boost);
            }
            this.occupiedDisplaySlots.Remove(TargetDisplaySlot);
    		this.TargetDisplaySlot.OccupiedRestocker = null;
            skill.AddExp(exp);
            yield break;
        }

        public void Internal_PlaceBox()
        {
            this.LogStat($"Called PlaceBox");
            if (this.Box == null)
            {
                return;
            }
            if (this.TargetRackSlot.IsBoxAlreadyExistInRack(this.Box.BoxID, this.Box))
            {
                return;
            }
            if (this.Box.Racked)
            {
                return;
            }
            Plugin.LogDebug($"Restocker[{skill.Id}] placing {this.Box?.ToBoxInfo()} on {this.TargetDisplaySlot}");
            this.Box.gameObject.layer = this.CurrentBoxLayer;
            Collider[] componentsInChildren = this.Box.GetComponentsInChildren<Collider>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].isTrigger = false;
            }
            this.TargetRackSlot.AddBox(this.Box.BoxID, this.Box);
            this.Box.Racked = true;
    		this.Box.SetOccupy(false, null);
    		this.TargetBox = null;

            this.inventory.Remove(this.Box);
            this.ArrangeBoxTower();

            var nextBox = this.inventory.Boxes.Where(b => b.Data.ProductID == TargetProductID).FirstOrDefault();
            if (nextBox == null)
            {
                nextBox = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts);
            }
            if (nextBox != null)
            {
                this.TargetProductID = nextBox.Data.ProductID;
            }
            this.Box = nextBox;
        }

        public bool Internal_GetAvailableDisplaySlotToRestock()
        {
            Plugin.LogDebug($"Restocker[{skill.Id}] called GetAvailableDisplaySlotToRestock");
            List<DisplaySlot> displaySlots = Singleton<DisplayManager>.Instance.GetDisplaySlots(this.TargetProductID, false);
            if (displaySlots == null || displaySlots.Count <= 0)
            {
                Plugin.LogDebug($"-> Not found");
                return false;
            }
            DisplaySlot displaySlot = displaySlots.FirstOrDefault(d => this.IsDisplaySlotAvailableToRestock(d));
            if (displaySlot == null)
            {
                Plugin.LogDebug($"-> finding labeledEmptyDisplaySlots");
                List<DisplaySlot> labeledEmptyDisplaySlots = Singleton<DisplayManager>.Instance.GetLabeledEmptyDisplaySlots(this.TargetProductID);
                if (labeledEmptyDisplaySlots == null || labeledEmptyDisplaySlots.Count <= 0)
                {
                    Plugin.LogDebug($"-> Not found");
                    return false;
                }
                DisplaySlot targetDisplaySlot = labeledEmptyDisplaySlots[UnityEngine.Random.Range(0, labeledEmptyDisplaySlots.Count)];
                this.TargetDisplaySlot = targetDisplaySlot;
            }
            else
            {
                Plugin.LogDebug($"-> Found: {displaySlot}");
                this.TargetDisplaySlot = displaySlot;
            }
            if (!this.occupiedDisplaySlots.Contains(this.TargetDisplaySlot))
            {
                this.occupiedDisplaySlots.Add(this.TargetDisplaySlot);
            }
    		this.TargetDisplaySlot.OccupiedRestocker = restocker;
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
            if (this.Box != null && this.Box.Data.ProductID == productSO.ID)
            {
                return !displaySlot.Full;
            }
            return true;
        }

        private void ArrangeBoxTower()
        {
            var tower = new List<Box>(this.inventory.Boxes);
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
            return this.inventory.Count > 0;
        }

        public void LogStat(string msg = null)
        {
            var call = msg != null ? $"{msg} " : ""; 
            Plugin.LogDebug($"Restocker[{skill.Id}] {call}carryingBox={this.Box}, boxCount={this.inventory.Count}");
            Plugin.LogDebug(this.inventory.Boxes.ToBoxStackInfo());
        }

        internal void AfterFreeTargetDisplaySlot()
        {
            this.occupiedDisplaySlots
                    .Where(s => s.IsOccupiedByMe(restocker))
                    .ForEach(s => s.OccupiedRestocker = null);
            this.occupiedDisplaySlots.Clear();
        }

        public class Inventory : List<InventorySlot>
        {
            public IEnumerable<Box> Boxes => this.Select(s => s.Box);

            public void Add(Box box)
            {
                this.Add(new InventorySlot(box));
            }

            public void Remove(Box box)
            {
                this.Remove(this.Find(s => s.Box.Equals(box)));
            }

            public IEnumerable<int> ProductIds => this.Where(s => s.Box.Data.ProductID != -1).Select(s => s.Box.Data.ProductID).Distinct();

        }

        public class InventorySlot
        {
            public Box Box;

            public InventorySlot(Box box)
            {
                this.Box = box;
            }
        }

    }
}