using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using EmployeeTraining.api;
using EmployeeTraining.Employee;
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

        private RestockerState State { get => this.fldState.Value; set => this.fldState.Value = value; }
        private readonly PrivateFld<RestockerState> fldState = new PrivateFld<RestockerState>(typeof(Restocker), "m_State");
        private int TargetProductID { get => this.fldTargetProductID.Value; set => this.fldTargetProductID.Value = value; }
        private readonly PrivateProp<int> fldTargetProductID = new PrivateProp<int>(typeof(Restocker), "m_TargetProductID");
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
        private List<DisplaySlot> CachedSlots { get => this.fldCashedSlots.Value; set => this.fldCashedSlots.Value = value; }
        private readonly PrivateFld<List<DisplaySlot>> fldCashedSlots = new PrivateFld<List<DisplaySlot>>(typeof(Restocker), "m_CachedSlots");
        private bool UsingVehicle { get => this.fldUsingVehicle.Value; set => this.fldUsingVehicle.Value = value; }
        private readonly PrivateFld<bool> fldUsingVehicle = new PrivateFld<bool>(typeof(Restocker), "m_UsingVehicle");
        private CharacterModelComponent ModelComponent { get => this.fldModelComponent.Value; set => this.fldModelComponent.Value = value; }
        private readonly PrivateFld<CharacterModelComponent> fldModelComponent = new PrivateFld<CharacterModelComponent>(typeof(Restocker), "m_ModelComponent");

        private Dictionary<int, List<RackSlot>> RackSlots { get => this.fldRackSlots.Value; set => this.fldRackSlots.Value = value; }
        private readonly PrivateFld<Dictionary<int, List<RackSlot>>> fldRackSlots = new PrivateFld<Dictionary<int, List<RackSlot>>>(typeof(RackManager), "m_RackSlots");
        private List<Rack> Racks { get => this.fldRacks.Value; set => this.fldRacks.Value = value; }
        private readonly PrivateFld<List<Rack>> fldRacks = new PrivateFld<List<Rack>>(typeof(RackManager), "m_Racks");
        // private Tween RackSlotColliderEnabler { get => this.fldRackSlotColliderEnabler.Value; set => this.fldRackSlotColliderEnabler.Value = value; }
        // private readonly PrivateFld<Tween> fldRackSlotColliderEnabler = new PrivateFld<Tween>(typeof(RackSlot), "m_ColliderEnabler");
        // private Highlightable RackSlotHighlightable { get => this.fldRackSlotHighlightable.Value; set => this.fldRackSlotHighlightable.Value = value; }
        // private readonly PrivateFld<Highlightable> fldRackSlotHighlightable = new PrivateFld<Highlightable>(typeof(RackSlot), "m_Highlightable");

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
        private readonly Func<int, int, bool> IsAvailableRackSlotToPlaceBox;
        private readonly PrivateMtd<bool> mtdIsAvailableRackSlotToPlaceBox = new PrivateMtd<bool>(typeof(Restocker), "IsAvailableRackSlotToPlaceBox", typeof(int), typeof(int));
        private readonly Func<Box, bool> HasEmptySpaceForMergeInAnyRack;
        private readonly PrivateMtd<bool> mtdHasEmptySpaceForMergeInAnyRack = new PrivateMtd<bool>(typeof(Restocker), "HasEmptySpaceForMergeInAnyRack", typeof(Box));
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
        private readonly Func<IEnumerator> PlaceBoxFromVehicle;
        private readonly PrivateMtd<IEnumerator> mtdPlaceBoxFromVehicle = new PrivateMtd<IEnumerator>(typeof(Restocker), "PlaceBoxFromVehicle");
        // private readonly PrivateMtd mtdRackSlotSetLabel = new PrivateMtd(typeof(RackSlot), "SetLabel");

        private readonly RestockerSkill skill;
        private readonly Restocker restocker;
        private readonly Inventory inventory = new Inventory();
        private readonly Dictionary<int, int> planList = new Dictionary<int, int>();
        private readonly Dictionary<int, int> carryingBoxes = new Dictionary<int, int>();
        private int productsNeeded;
        private int totalCarryingWeight = 0;
        private int totalCarryingHeight = 0;
        private readonly List<DisplaySlot> occupiedDisplaySlots = new List<DisplaySlot>();
        private readonly List<DisplaySlot> labeledEmptySlotsCache = new List<DisplaySlot>(250);
        
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

        private static bool VerboseLog => Plugin.Instance.Settings.RestockerLog;
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
            this.fldAgent.Instance = restocker;
            this.fldIsCarryBoxToRack.Instance = restocker;
            this.fldMinFillRateForDisplaySlotsToRestock.Instance = restocker;
            this.fldTargetBox.Instance = restocker;
            this.fldCurrentBoostLevel.Instance = restocker;
            this.fldRestockerWalkingSpeeds.Instance = restocker;
            this.fldRestockerPlacingSpeeds.Instance = restocker;
            this.fldCashedSlots.Instance = restocker;
            this.fldUsingVehicle.Instance = restocker;
            this.fldModelComponent.Instance = restocker;

            this.fldRackSlots.Instance = Singleton<RackManager>.Instance;
            this.fldRacks.Instance = Singleton<RackManager>.Instance;

            this.ResetTargets = () => this.mtdResetTargets.Invoke();
            this.mtdResetTargets.Instance = restocker;
            this.TryRestocking = () => this.mtdTryRestocking.Invoke();
            this.mtdTryRestocking.Instance = restocker;
            this.PlaceBoxFromStreet = () => this.mtdPlaceBoxFromStreet.Invoke();
            this.mtdPlaceBoxFromStreet.Instance = restocker;
            this.PlaceBox = () => this.mtdPlaceBox.Invoke();
            this.mtdPlaceBox.Instance = restocker;
            this.DropBox = () => this.mtdDropBox.Invoke();
            this.mtdDropBox.Instance = restocker;
            this.PickUpBox = (isFromRack) => this.mtdPickUpBox.Invoke(isFromRack);
            this.mtdPickUpBox.Instance = restocker;
            this.PerformRestocking = () => this.mtdPerformRestocking.Invoke();
            this.mtdPerformRestocking.Instance = restocker;
            this.PlaceProducts = () => this.mtdPlaceProducts.Invoke();
            this.mtdPlaceProducts.Instance = restocker;
            this.PlaceBoxToRack = () => this.mtdPlaceBoxToRack.Invoke();
            this.mtdPlaceBoxToRack.Instance = restocker;
            this.GetAvailableDisplaySlotToRestock = () => this.mtdGetAvailableDisplaySlotToRestock.Invoke();
            this.mtdGetAvailableDisplaySlotToRestock.Instance = restocker;
            this.CheckForAvailableRackSlotToTakeBox = () => this.mtdCheckForAvailableRackSlotToTakeBox.Invoke();
            this.mtdCheckForAvailableRackSlotToTakeBox.Instance = restocker;
            this.CheckForAvailableRackSlotToPlaceBox = () => this.mtdCheckForAvailableRackSlotToPlaceBox.Invoke();
            this.mtdCheckForAvailableRackSlotToPlaceBox.Instance = restocker;
            // this.IsDisplaySlotAvailableToRestock = (displaySlot) => mtdIsDisplaySlotAvailableToRestock.Invoke(displaySlot);
            // this.mtdIsDisplaySlotAvailableToRestock.Instance = restocker;
            this.IsAvailableRackSlotToPlaceBox = (productID, boxID) => this.mtdIsAvailableRackSlotToPlaceBox.Invoke(productID, boxID);
            this.mtdIsAvailableRackSlotToPlaceBox.Instance = restocker;
            this.HasEmptySpaceForMergeInAnyRack = (box) => this.mtdHasEmptySpaceForMergeInAnyRack.Invoke(box);
            this.mtdHasEmptySpaceForMergeInAnyRack.Instance = restocker;
            this.IsRackSlotStillAvailable = (rackSlot, productId) => this.mtdIsRackSlotStillAvailable.Invoke(rackSlot, productId);
            this.mtdIsRackSlotStillAvailable.Instance = restocker;
            this.ThrowBoxToTrashBin = () => this.mtdThrowBoxToTrashBin.Invoke();
            this.mtdThrowBoxToTrashBin.Instance = restocker;
            this.GoTo = (position, rotation) => this.mtdGoTo.Invoke(position, rotation);
            this.mtdGoTo.Instance = restocker;
            this.GoToWaiting = (state) => this.mtdGoToWaiting.Invoke(state);
            this.mtdGoToWaiting.Instance = restocker;
            this.HasBoxAtRackForMerge = (box) => this.mtdHasBoxAtRackForMerge.Invoke(box);
            this.mtdHasBoxAtRackForMerge.Instance = restocker;
            this.MergeBox = (slot) => this.mtdMergeBox.Invoke(slot);
            this.mtdMergeBox.Instance = restocker;
            this.PlaceBoxFromVehicle = () => this.mtdPlaceBoxFromVehicle.Invoke();
            this.mtdPlaceBoxFromVehicle.Instance = restocker;
        }

        public void AfterResetRestocker()
        {
            this.UnoccupyBoxes();
            this.inventory.Clear();
            this.carryingBoxes.Clear();
            this.planList.Clear();
            this.UpdateCarryingWeightAndHeight();
        }

        public void UnoccupyBoxes()
        {
            // TODO: List locked boxes to unlock em all
            this.inventory.ForEach(s => s.Box.SetOccupy(false, null));
            // Task.Run(() => {
            //     Box[] boxes = UnityEngine.Object.FindObjectsOfType<Box>();
            //     boxes.Where(b => b.OccupyOwner == this.restocker.transform).ForEach(b => b.SetOccupy(false, null));
            // });
        }

        public void Internal_DropTheBox()
        {
            // this.LogStat("Called DropTheBox");
            // Singleton<EmployeeManager>.Instance.OccupyProductID(this.TargetProductID, false);
            if (this.Box == null || !this.restocker.CarryingBox)
            {
                return;
            }
            foreach (InventorySlot slot in this.inventory)
            {
                var box = slot.Box;
                Singleton<InventoryManager>.Instance.RemoveBox(box.Data);
                LeanPool.Despawn(box.gameObject);
                box.gameObject.layer = slot.Layer;
                box.ResetBox();
            }
            this.UnoccupyBoxes();
            this.inventory.Clear();
            this.carryingBoxes.Clear();
            this.planList.Clear();
            this.UpdateCarryingWeightAndHeight();
            this.Box = null;
            this.TargetBox = null;
            this.restocker.CarryingBox = false;
            this.State = RestockerState.IDLE;
            this.CheckTasks = true;
        }

        public void Internal_DropBoxToGround()
        {
            foreach (InventorySlot slot in this.inventory)
            {
                slot.Box.DropBox();
                slot.Box.gameObject.layer = slot.Layer;
                // The vanilla somehow doesn't do this and the box is abandoned...
                Singleton<StorageStreet>.Instance.SubscribeBox(slot.Box);
            }
            this.UnoccupyBoxes();
            this.inventory.Clear();
            this.carryingBoxes.Clear();
            this.planList.Clear();
            this.UpdateCarryingWeightAndHeight();
            this.Box = null;
            this.TargetBox = null;
            this.restocker.CarryingBox = false;
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
            Singleton<InventoryManager>.Instance.UpdateOrderedProducts();
            this.planList.Clear();
            this.carryingBoxes.Clear();
            this.MinFillRateForDisplaySlotsToRestock = 1;
            this.UpdateCarryingWeightAndHeight();
            bool doneRestocking = false;

            if (restocker.ManagementData.RestockFromVehicles)
            {
                yield return this.PlaceBoxFromVehicle();
                if (this.UsingVehicle)
                {
                    yield break;
                }
            }

            List<int> productsToRestock = this.MakeAPlanToRestock();

            this.restocker.FreeTargetDisplaySlot();
            yield return null;

            for (int j = 0; j < productsToRestock.Count; j++)
            {
                this.TargetProductID = productsToRestock[j];
                this.productsNeeded = this.GetTotalDisplayCapacity(this.TargetProductID) - Singleton<DisplayManager>.Instance.GetDisplayedProductCount(this.TargetProductID);
                // bool pickedUp = false;
                this.LogStat($"Collecting goods for {Singleton<IDManager>.Instance.ProductSO(this.TargetProductID).name} x {this.productsNeeded}");
                while (this.productsNeeded > 0 && this.GetAvailableDisplaySlotToRestock() && this.restocker.ManagementData.RestockShelf)
                {
                    this.CheckTasks = false;
                    bool isBoxFromRack = true;
                    if (this.restocker.ManagementData.PickUpBoxGround)
                    {
                        this.LogSimple($"Trying to get a box from street");
                        this.TargetBox = this.GetBoxFromStreet();
                    }
                    if (this.TargetBox != null && this.restocker.ManagementData.PickUpBoxGround)
                    {
                        this.LogSimple($"Found a box and aiming for TargetBox={this.TargetBox?.ToBoxInfo() ?? "NULL"}");
                        this.TargetBox.SetOccupy(true, this.restocker.transform);
                        isBoxFromRack = false;
                        Vector3 target = Vector3.MoveTowards(this.TargetBox.transform.position, this.restocker.transform.position, 0.35f);
                        Vector3 position = this.TargetBox.transform.position;
                        position.y = this.restocker.transform.position.y;
                        Quaternion rotation = Quaternion.LookRotation(position, Vector3.up);
                        yield return this.restocker.StartCoroutine(this.GoTo(target, rotation));
                    }
                    else
                    {
                        this.TargetBox = null;
                        bool foundAvailableRack = false;
                        while (!foundAvailableRack && this.CheckForAvailableRackSlotToTakeBox())
                        {
                            this.LogStat($"going to the rack {this.TargetRackSlot.ToRackInfo()}");
                            yield return this.restocker.StartCoroutine(this.GoTo(this.TargetRackSlot.InteractionPosition, this.TargetRackSlot.InteractionRotation));
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
                        this.occupiedDisplaySlots.Remove(this.TargetDisplaySlot);
                        this.LogSimple($"Trying to get another display slot");
                        if (!this.GetAvailableDisplaySlotToRestock())
                        {
                            break;
                        }
                    }
                    // TargetBox is null or (not null and) not occupied or occupied by me)
                    this.LogSimple($"Trying to pick up TargetBox={this.TargetBox?.ToBoxInfo() ?? "NULL"}");
                    if (this.TargetBox == null || !this.TargetBox.IsBoxOccupied || this.TargetBox.OccupyOwner == this.restocker.transform)
                    {
                        yield return this.restocker.StartCoroutine(this.PickUpBox(isBoxFromRack));
                        if (isBoxFromRack && this.TargetRackSlot != null && this.restocker.ManagementData.RemoveLabelRack && !this.TargetRackSlot.HasBox)
                        {
                            this.TargetRackSlot.ClearLabel();
                        }
                    }
                }
            }

            this.LogStat("has finished collection, will restock");
            var productIds = this.inventory.ProductIds.ToList();
            foreach (int id in productIds)
            {
                this.TargetProductID = id;
                //this.GetAvailableDisplaySlotToRestock();

                // Get every available slot to restock
                Singleton<DisplayManager>.Instance.GetDisplaySlots(this.TargetProductID, false, this.CachedSlots);
                Singleton<DisplayManager>.Instance.GetLabeledEmptyDisplaySlots(this.TargetProductID, this.labeledEmptySlotsCache);
                this.CachedSlots.AddRange(this.labeledEmptySlotsCache);

                this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
                bool foundAvailableDisplaySlot = false;
                // Collection was modified exception can happen within this loop.
                // However, copying the CashedSlots is not a good idea, as it causes the hanging boxes in the air :/
                foreach (DisplaySlot slot in this.CachedSlots)
                {
                    this.TargetDisplaySlot = slot;
                    if (!this.IsDisplaySlotAvailableToRestock(slot))
                    {
                        continue;
                    }
                    if (this.TargetBox != null && !this.TargetBox.HasProducts
                        || this.TargetBox != null && this.TargetBox.OccupyOwner != this.restocker.transform)
                    {
                        break;
                    }
                    this.LogStat($"going to the display {this.TargetDisplaySlot}");
                    this.TargetDisplaySlot.OccupiedRestocker = this.restocker;
                    this.occupiedDisplaySlots.Add(this.TargetDisplaySlot);
                    yield return this.restocker.StartCoroutine(
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
                    if (this.TargetDisplaySlot)
                    {
                        this.TargetDisplaySlot.OccupiedRestocker = null;
                        this.occupiedDisplaySlots.Remove(this.TargetDisplaySlot);
                    }
                }

                if (!foundAvailableDisplaySlot)
                {
                    if (this.TargetDisplaySlot)
                    {
                        this.TargetDisplaySlot.OccupiedRestocker = null;
                    }
                    this.occupiedDisplaySlots.Remove(this.TargetDisplaySlot);
                }
                else
                {
                    this.LogStat($"restocking to {this.TargetDisplaySlot}");
                    yield return this.restocker.StartCoroutine(this.PerformRestocking());
                    this.LogStat($"done restocking");
                }
                doneRestocking = true;
            }

            this.LogStat($"goes dropping box");
            yield return this.restocker.StartCoroutine(this.DropBox());
            if (doneRestocking)
            {
                this.skill.AddExp(2);
                yield break;
            }

            if (!this.HasBox())
            {
                this.restocker.FreeTargetDisplaySlot();
                this.planList.Clear();
                this.IsCarryBoxToRack = false;
                if (this.restocker.ManagementData.PickUpBoxGround)
                {
                    yield return this.restocker.StartCoroutine(this.PlaceBoxFromStreet());
                }
                if (!this.restocker.CarryingBox && !this.IsCarryBoxToRack && this.State != RestockerState.IDLE)
                {
                    yield return this.restocker.StartCoroutine(this.restocker.SoftResetRestocker());
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
            bool includeEmptyBox = this.restocker.ManagementData.PickUpBoxGround;
            List<Box> boxes = boxesOnStreet.FindAll(x => x.HasProducts && !x.Racked && x.Product.ID == this.TargetProductID
                && x.gameObject.activeInHierarchy
                && (!x.IsBoxOccupied || x.IsBoxOccupied && x.OccupyOwner == this.restocker.transform));
            if (includeEmptyBox)
            {
                boxes.AddRange(boxesOnStreet.FindAll(x => !x.HasProducts
                    && (!x.IsBoxOccupied || x.IsBoxOccupied && x.OccupyOwner == this.restocker.transform)));
            }
            
            return boxes.Count > 0 ? boxes.GetRandom() : null;
        }

        private List<int> MakeAPlanToRestock()
        {
            var customers = Singleton<ShoppingCustomerList>.Instance.CustomersInShopping;
            bool isCustomerInShopping = customers.Count > 0 && customers.Any(c => c.ShoppingList != null && c.ShoppingList.HasProduct);
            bool canFullyStock = !isCustomerInShopping;
            if (RestockerLogic.VerboseLog)
            {
                Plugin.LogDebug($"Restocker[{skill.Id}] canFullyStock: {canFullyStock}");
                Plugin.LogDebug($"ActiveCustomers={customers.Count}, {customers.Select(c => $"[{c.ShoppingList?.Products.Keys.Join()}]").Join()}");
            }

            var dispMgr = Singleton<DisplayManager>.Instance;
            var carrying = this.CollectProductsCarrying();
            var demands = (from i in dispMgr.DisplayedProducts.AsParallel()
                    where i.Value.Count > 0
                    let capacity = this.GetTotalDisplayCapacity(i.Key)
                    let displayed = dispMgr.GetDisplayedProductCount(i.Key)
                    let stocking = displayed + carrying.Get(i.Key, 0)
                    where stocking < capacity * (canFullyStock ? 1 : 0.8f)
                    orderby stocking ascending
                    select new KeyValuePair<int, int>(i.Key, capacity - displayed - carrying.Get(i.Key, 0)))
                    .ToList();
            if (RestockerLogic.VerboseLog)
            {
                Plugin.LogDebug($"Restocker[{this.restocker.RestockerID}] Demands: {demands.Select(p => $"[{Singleton<IDManager>.Instance.ProductSO(p.Key).name} x{p.Value}]").Join(delimiter: "")}");
                // Plugin.LogDebug($"Products in a box:");
                // Singleton<IDManager>.Instance.Products.ForEach(p => Plugin.LogDebug($"{p.ID}\t{p.GridLayoutInBox.productCount}"));
            }

            int totalWeightOfPlan = 0;
            int totalHeightOfPlan = 0;
            foreach (KeyValuePair<int, int> p in demands)
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
            if (RestockerLogic.VerboseLog)
            {
                this.LogSimple($"Planned: {this.planList.Select(p => $"[{Singleton<IDManager>.Instance.ProductSO(p.Key).name} x{p.Value}]").Join(delimiter: "")}");
                this.LogSimple($"Total weight: {totalWeightOfPlan}, Capacity: {this.CarryingCapacity}");
            }

            return this.planList.Select(p => p.Key).ToList();
        }

        private int GetTotalDisplayCapacity(int productId)
        {
            var slots = new List<DisplaySlot>();
            Singleton<DisplayManager>.Instance.GetDisplaySlots(productId, false, slots);
            return slots.Distinct().Sum(i => this.GetCapacityInDisplaySlot(i));
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
            List<int> idList = this.restocker.GetAvailableProductIDList();
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

        private Dictionary<int, int> CollectBoxesBeingCarried()
        {
            return (from k in from i in RestockerSkillManager.Instance.GetActiveLogics()
                from j in i.carryingBoxes select j
                    group k.Value by k.Key)
                .ToDictionary(g => g.Key, g => g.Sum());
        }

        public List<int> Internal_GetAvailableProductIDList()
        {
            List<int> list = new List<int>();
            foreach (Box item in Singleton<StorageStreet>.Instance.GetBoxesFromStreet())
            {
                if (item != null && item.Product != null && item.Product.ID > -1)
                {
                    if (this.IsAvailableRackSlotToPlaceBox(item.Data.ProductID, item.BoxID))
                    {
                        list.Add(item.Data.ProductID);
                    }
                    else if (this.HasEmptySpaceForMergeInAnyRack(item))
                    {
                        list.Add(item.Data.ProductID);
                    }
                }
            }

            return list;
        }

        public IEnumerator Internal_PlaceBoxFromVehicle()
        {
            this.UsingVehicle = false;
            List<Box> boxesToCarry = new List<Box>();
            var totalCarryingBoxes = this.CollectBoxesBeingCarried();

            foreach (GameObject vehicleObj in Singleton<VehicleManager>.Instance.GetVehicles())
            {
                if (vehicleObj && vehicleObj.activeInHierarchy
                    && Singleton<StorageStreet>.Instance.IsWithinRestockableArea(vehicleObj.transform.position)
                    && (!vehicleObj.TryGetComponent<VehicleRigidbodyStopDuration>(out VehicleRigidbodyStopDuration vehicleRigidbodyStopDuration) || vehicleRigidbodyStopDuration.HasStopped))
                {
                    IPlacementArea componentInChildren = vehicleObj.GetComponentInChildren<IPlacementArea>();
                    if (componentInChildren != null)
                    {
                        foreach (SortableBox sortableBox in componentInChildren.GetBoxes())
                        {
                            if (sortableBox && sortableBox.TryGetComponent<RestockAreaBoxController>(out RestockAreaBoxController restockAreaBoxController)
                                && restockAreaBoxController.HasLeftArea && sortableBox.TryGetComponent<Box>(out Box box))
                            {
                                if (!box.IsBoxOccupied && !box.Racked && box.HasProducts && !this.IsBoxOvercapacity(box))
                                {
                                    int pid = box.Data.ProductID;
                                    int productBoxCnt = boxesToCarry.Where(b => b.Data.ProductID == pid).Count();
                                    if (productBoxCnt < this.GetRackCapacityOfSpaceFor(pid) - totalCarryingBoxes.Get(pid, 0))
                                    {
                                        this.TargetBox = box;
                                        this.Box = this.TargetBox;
                                        this.TargetProductID = this.Box.Product.ID;
                                        box.SetOccupy(true, restocker.transform);
                                        boxesToCarry.Add(box);
                                        this.AddCarryingBox(box);
                                    }
                                }
                                this.TargetBox = null;
                            }
                        }
                    }
                }
            }
            foreach (Box box in boxesToCarry)
            {
                this.TargetBox = box;
                this.Box = box;
                this.UsingVehicle = true;
                Vector3 target = this.TargetBox.transform.position - (this.TargetBox.transform.position - restocker.transform.position).normalized * 1f;
                Quaternion rotation = Quaternion.LookRotation(this.TargetBox.transform.position, Vector3.up);
                yield return this.GoTo(target, rotation);
                if (this.Box.OccupyOwner != restocker.transform)
                {
                    this.State = RestockerState.IDLE;
                    this.CheckTasks = true;
                    this.TargetBox = null;
                    this.Box = null;
                    continue;
                }
                this.CheckTasks = false;
                yield return this.PickUpBox(false);
                this.GetAvailableDisplaySlotToRestock();
                this.CheckTasks = true;
            }
            yield return this.DropBox();
            yield return null;
        }

        public IEnumerator Internal_PlaceBoxFromStreet()
        {
            this.LogStat($"called PlaceBoxFromStreet");
            this.IsCarryBoxToRack = false;
            List<Box> boxesToCarry = new List<Box>();
            var totalCarryingBoxes = this.CollectBoxesBeingCarried();
            foreach (Box box in this.GetBoxListOnStreet())
            {
                if (box == null || box.IsBoxOccupied && box.OccupyOwner != this.restocker.transform
                        || box.Racked || !box.HasProducts || this.IsBoxOvercapacity(box))
                {
                    continue;
                }
                int pid = box.Data.ProductID;
                int productBoxCnt = boxesToCarry.Where(b => b.Data.ProductID == pid).Count();
                if (productBoxCnt < this.GetRackCapacityOfSpaceFor(pid) - totalCarryingBoxes.Get(pid, 0))
                {
                    box.SetOccupy(true, this.restocker.transform);
                    boxesToCarry.Add(box);
                    this.AddCarryingBox(box);
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
                Vector3 target = Vector3.MoveTowards(this.TargetBox.transform.position, this.restocker.transform.position, 0.35f);
                Quaternion rotation = Quaternion.LookRotation(this.TargetBox.transform.position, Vector3.up);
                yield return this.restocker.StartCoroutine(this.GoTo(target, rotation));
                if (!this.TargetBox || this.TargetBox.Racked)
                {
                    this.DoneCarryingBox(this.TargetBox);
                    this.TargetBox = null;
                    this.TargetProductID = -1;
                }
                else
                {
                    this.LogStat($"picking up from street: {this.TargetBox.ToBoxInfo()}");
                    yield return this.restocker.StartCoroutine(this.PickUpBox(false));
                    if (this.HasBox())
                    {
                        this.IsCarryBoxToRack = true;
                    }
                }
            }

            yield return this.DropBox();
            if (this.IsCarryBoxToRack)
            {
                this.skill.AddExp(2);
            }

            this.LogStat($"finished PlaceBoxFromStreet");
        }

        public IEnumerator Internal_DropBox()
        {
            // this.LogStat($"Called DropBox");
            if (this.TargetDisplaySlot != null && this.TargetDisplaySlot.IsOccupiedByOthers(this.restocker))
            {
                this.occupiedDisplaySlots.Remove(this.TargetDisplaySlot);
                this.TargetDisplaySlot.OccupiedRestocker = null;
            }
            if (!this.HasBox())
            {
                this.UpdateCarryingWeightAndHeight();
                yield break;
            }
            if (this.inventory.Boxes.Any(b => !b.HasProducts))
            {
                yield return this.restocker.StartCoroutine(this.ThrowBoxToTrashBin());
            }
            if (this.inventory.Boxes.Any(b => b.HasProducts))
            {
                yield return this.restocker.StartCoroutine(this.PlaceBoxToRack());
            }
            this.UpdateCarryingWeightAndHeight();
        }

        public IEnumerator Internal_PerformRestocking()
        {
            this.DoneRestocking(this.Box);
            yield return this.restocker.StartCoroutine(this.PlaceProducts());
            this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
            while (this.Box != null && this.Box.HasProducts && this.GetAvailableDisplaySlotToRestock())
            {
                this.LogSimple($"Trying to restock {this.Box.ToBoxInfo()}");
                yield return this.restocker.StartCoroutine(this.GoTo(
                        this.TargetDisplaySlot.InteractionPosition - this.TargetDisplaySlot.InteractionPositionForward * 0.3f,
                        this.TargetDisplaySlot.InteractionRotation));
                this.DoneRestocking(this.Box);
                if (this.TargetDisplaySlot == null
                        || !this.TargetDisplaySlot.gameObject.activeInHierarchy
                        || this.TargetDisplaySlot.Full
                        || !this.IsDisplaySlotAvailableToRestock(this.TargetDisplaySlot)
                        || this.TargetProductID != this.TargetDisplaySlot.ProductID)
                {
                    this.occupiedDisplaySlots.Remove(this.TargetDisplaySlot);
                    this.TargetDisplaySlot.OccupiedRestocker = null;
                }
                else
                {
                    this.LogStat("calling PlaceProducts");
                    yield return this.restocker.StartCoroutine(this.PlaceProducts());
                }
                this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts && b.Data.ProductID == this.TargetProductID);
            }
            if (this.State == RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT)
            {
                this.restocker.StartCoroutine(this.DropBox());
            }
        }

        public IEnumerator Internal_PlaceBoxToRack()
        {
            this.LogStat($"called PlaceBoxToRack");
            this.Box = this.inventory.Boxes.FirstOrDefault(b => b.HasProducts);
            if (this.Box == null)
            {
                this.LogSimple($"No box to place");
                if (this.inventory.Boxes.Any(b => !b.HasProducts))
                {
                    yield return this.restocker.StartCoroutine(this.ThrowBoxToTrashBin());
                }
                this.restocker.CarryingBox = false;
                this.State = RestockerState.IDLE;
                this.restocker.StartCoroutine(this.TryRestocking());
                yield break;
            }

            this.TargetProductID = this.Box.Data.ProductID;

            while (this.restocker.CarryingBox)
            {
                RackSlot rackSlot = this.HasBoxAtRackForMerge(this.Box);
                this.LogStat($"trying to merge {this.Box?.ToBoxInfo()} to rack slot");
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
                    yield return this.restocker.StartCoroutine(
                            this.GoTo(this.TargetRackSlot.InteractionPosition, this.TargetRackSlot.InteractionRotation));
                    
                    if (!(this.TargetRackSlot == null) 
                            && this.TargetRackSlot.gameObject.activeInHierarchy 
                            && !this.TargetRackSlot.Full 
                            && (!this.TargetRackSlot.HasProduct
                                || this.TargetRackSlot.Data.ProductID == this.TargetProductID)
                            && (this.TargetRackSlot.Data.ProductID == -1
                                || this.TargetRackSlot.Data.ProductID == this.TargetProductID)
                            && (this.TargetRackSlot.HasProduct || !this.TargetRackSlot.HasBox)
                            && (this.restocker.ManagementData.UseUnlabeledRacks || TargetRackSlot.HasLabel))
                    {
                        this.LogStat($"placing box {this.Box?.ToBoxInfo()}");
                        this.DoneCarryingBox(this.Box);
                        this.PlaceBox();
                    }
                }
                if (!this.HasBox() || !this.inventory.Boxes.Any(b => b.HasProducts))
                {
                    if (this.inventory.Boxes.Any(b => !b.HasProducts))
                    {
                        yield return this.restocker.StartCoroutine(this.ThrowBoxToTrashBin());
                    }

                    this.LogStat($"done placing box");
                    this.restocker.CarryingBox = false;
                    this.State = RestockerState.IDLE;
                    this.restocker.StartCoroutine(this.TryRestocking());
                    yield break;
                }
            }

            if (this.restocker.CarryingBox)
            {
                this.LogStat($"not done placing box, wating for rack to place");
                this.restocker.StartCoroutine(this.GoToWaiting(RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT));
            }
        }

        public IEnumerator Internal_ThrowBoxToTrashBin()
        {
            this.LogStat($"Called ThrowBoxToTrashBin");
            yield return this.restocker.StartCoroutine(this.GoTo(
                Singleton<FurnitureManager>.Instance.TrashBin.position,
                Singleton<FurnitureManager>.Instance.TrashBin.rotation));

            this.Box = this.inventory.Boxes.FirstOrDefault(b => !b.HasProducts);

            while (this.Box != null)
            {
                yield return new WaitForSeconds(this.ThrowingBoxTime);
                // this.LogStat();
                RestockerEventApi.BoxThrownIntoTrashEventRegistry.Invoke(this.restocker, this.Box);
                Singleton<InventoryManager>.Instance.RemoveBox(this.Box.Data);
                LeanPool.Despawn(this.Box.gameObject);
                this.Box.gameObject.layer = this.inventory.BoxLayer(this.Box);
                this.Box.ResetBox();
                this.inventory.Remove(this.Box);
                this.ArrangeBoxTower();
                this.DoneCarryingBox(this.Box);
                this.Box = this.inventory.Boxes.FirstOrDefault(b => !b.HasProducts);
            }
            this.LogStat("threw boxes to trash bin");

            this.Box = this.inventory.Boxes.FirstOrDefault();
            // this.LogStat();
            if (this.Box == null)
            {
                this.TargetBox = null;
                this.restocker.CarryingBox = false;
                this.State = RestockerState.IDLE;
                // Singleton<EmployeeManager>.Instance.OccupyProductID(this.TargetProductID, false);
                this.restocker.StartCoroutine(this.TryRestocking());
            }
            else if (this.State == RestockerState.WAITING_FOR_AVAILABLE_RACK_SLOT)
            {
                this.LogSimple($"waiting for avalilable rack slot");
                this.restocker.StartCoroutine(this.PlaceBoxToRack());
            }
        }

        public IEnumerator Internal_MoveTo(Vector3 target)
        {
            var boost = this.RestockerWalkingSpeeds[this.CurrentBoostLevel] / 2f; // 2f when not boosted
            var speed = this.MovingSpeed * boost;
            this.Agent.speed = speed;
            this.Agent.angularSpeed = this.AngularSpeed * boost;
            this.Agent.acceleration = this.Acceleration * boost;
            // Plugin.LogDebug($"Agent: speed={this.Agent.speed}, angularSpeed={this.Agent.angularSpeed}, acceleration={this.Agent.acceleration}");

            yield return EmployeeLogicHelper.MoveTo(this.restocker, target, this.Agent, boost, this.TurningSpeed, 20f);
        }

        public IEnumerator Internal_RotateTo(Quaternion rotation)
        {
            this.restocker.transform.DORotateQuaternion(rotation, this.RotationTime);
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
                    if (this.IsBoxOvercapacity(nextBoxData))
                    {
                        this.LogSimple($"Overcapacity! {this.productsNeeded} of needs decreasing by {nextBoxData.ProductCount}");
                        this.productsNeeded -= nextBoxData.ProductCount;
                        this.DoneRestocking(nextBoxData);
                        continue;
                    }
                    Box box = this.TargetRackSlot.TakeBoxFromRack();
                    if (box == null)
                    {
                        yield break;
                    }
                    if (this.inventory.Contains(box))
                    {
                        this.LogSimple($"already picked up {box.ToBoxInfo()}");
                        yield break;
                    }

                    if (box.OnPlacementArea && box.TryGetComponent<SortableBox>(out SortableBox sortableBox))
                    {
                        box.GetComponentInParent<IPlacementArea>().RemoveBox(sortableBox);
                    }
                    if (!this.carryingBoxes.TryAdd(box.Data.ProductID, 1))
                    {
                        this.carryingBoxes[box.Data.ProductID] += 1;
                    }
                    this.AddCarryingBox(box);
                    this.LogStat($"picking up {box.ToBoxInfo()} from a rack");
                    yield return this.GrabBox(box, isFromRack);
                }
            }
            else
            {
                // From street or vehicles
                if (this.TargetBox == null) yield break;

                if (this.TargetBox.OccupyOwner != this.restocker.transform)
                {
                    this.TargetBox = null;
                    yield break;
                }
                if (this.inventory.Contains(this.TargetBox))
                {
                    this.LogSimple($"already picked up {this.TargetBox.ToBoxInfo()}");
                    yield break;
                }
                this.LogStat($"picking up {this.TargetBox?.ToBoxInfo()} from the ground or vehicles");
                if (this.TargetBox.OnPlacementArea && this.TargetBox.TryGetComponent<SortableBox>(out SortableBox sortableBox))
                {
                    this.TargetBox.GetComponentInParent<IPlacementArea>().RemoveBox(sortableBox);
                }
                yield return this.GrabBox(this.TargetBox, isFromRack);
            }
        }

        private IEnumerator GrabBox(Box box, bool isFromRack)
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
            if (this.ModelComponent.TryGetReference("BoxHolder", out CharacterModelObjectReference characterModelObjectReference))
            {
                box.transform.SetParent(characterModelObjectReference.transform);
                box.transform.DOLocalMove(Vector3.zero, this.TakingBoxTime, false);
                box.transform.DOLocalRotate(Vector3.zero, this.TakingBoxTime, RotateMode.Fast);
            }
            this.ArrangeBoxTower();
            this.Box = box;
            this.Box.SetOccupy(true, this.restocker.transform);
            this.Box.Racked = false;
            this.CurrentBoxLayer = box.gameObject.layer;
            box.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            this.restocker.CarryingBox = true;
            yield return new WaitForSeconds(this.TakingBoxTime);
        }

        private void DoneRestocking(Box box)
        {
            if (box == null) return;
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
            this.LogSimple($"Called PlaceProducts(): Box={this.Box?.ToBoxInfo()}, TargetProductID={this.TargetProductID}, TargetDisplaySlot={this.TargetDisplaySlot}");
            if (this.Box == null || this.TargetDisplaySlot == null || this.TargetProductID != this.TargetDisplaySlot.ProductID)
            {
                this.LogSimple($"Not passed validation");
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
                this.LogSimple($"Tried to place wrong product ID");
                yield break;
            }
            int exp = 0;
            while (TargetDisplaySlot && !this.TargetDisplaySlot.Full && this.Box.HasProducts)
            {
                Product productFromBox = null;
                try
                {
                    productFromBox = this.Box.GetProductFromBox();
                    this.LogSimple($"Placing a product from the box {this.Box.ToBoxInfo()}");
                }
                catch (ArgumentOutOfRangeException) { } // It can happen accidentally...
                if (productFromBox == null)
                {
                    break;
                }

                if (productFromBox.ProductSO.ID != this.TargetDisplaySlot.ProductID)
                {
                    this.LogSimple($"Tried to mix the wrong product");
                    break;
                    // Drop product from the box
                    // this.LogSimple($"Tried to mix the wrong product! {productFromBox.ProductSO.name} slapped down to the floor");
                    // productFromBox.transform.parent = null;
                    // productFromBox.transform.DOKill(false);
                    // productFromBox.SpreadOn(productFromBox.ProductSO.ID, out var _rb);
                    // _rb.AddForce(2f * new Vector3(UnityEngine.Random.Range(-22f, 22f), 45f, UnityEngine.Random.Range(-22f, 22f)));
                    // continue;
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
            this.occupiedDisplaySlots.Remove(this.TargetDisplaySlot);
            this.TargetDisplaySlot.OccupiedRestocker = null;
            this.skill.AddExp(exp);
            this.LogSimple($"Finished PlaceProducts()");
        }

        public void Internal_PlaceBox()
        {
            this.LogStat($"Called PlaceBox");
            if (this.Box == null)
            {
                return;
            }
            if (this.TargetRackSlot.Data.ProductID != -1 && this.TargetRackSlot.Data.ProductID != this.TargetProductID)
            {
                this.LogSimple($"tried to place box with wrong product ID: {this.TargetRackSlot.Data.ProductID} != {this.TargetProductID}");
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
            this.LogSimple($"placing {this.Box?.ToBoxInfo() ?? "[EMPTY]"} on {this.TargetDisplaySlot}");
            this.Box.gameObject.layer = this.inventory.BoxLayer(this.Box);
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

            var nextBox = this.inventory.Boxes.Where(b => b.Data.ProductID == this.TargetProductID).FirstOrDefault();
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
            this.LogSimple($"called GetAvailableDisplaySlotToRestock");
            if (Singleton<DisplayManager>.Instance.GetDisplaySlots(this.TargetProductID, false, this.CachedSlots) <= 0)
            {
                Plugin.LogDebug($"-> Not found");
                return false;
            }

            // Instead of GetAvailableDisplaySlotsNonAlloc()
            DisplaySlot displaySlot = this.CachedSlots.FirstOrDefault(d => this.IsDisplaySlotAvailableToRestock(d));
            if (displaySlot == null)
            {
                Plugin.LogDebug($"-> finding labeledEmptyDisplaySlots");
                if (Singleton<DisplayManager>.Instance.GetLabeledEmptyDisplaySlots(this.TargetProductID, this.labeledEmptySlotsCache) <= 0)
                {
                    Plugin.LogDebug($"-> Not found");
                    return false;
                }
                DisplaySlot targetDisplaySlot = this.CachedSlots[UnityEngine.Random.Range(0, this.labeledEmptySlotsCache.Count)];
                if (targetDisplaySlot.IsOccupiedByOthers(restocker))
                {
                    return false;
                }
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
            this.TargetDisplaySlot.OccupiedRestocker = this.restocker;
            return true;
        }

        public bool IsDisplaySlotAvailableToRestock(DisplaySlot displaySlot)
        {
            if (displaySlot.Data == null || displaySlot.Data.FirstItemID <= 0
                    || displaySlot.IsOccupiedByOthers(this.restocker))
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

        private bool IsBoxOvercapacity(BoxData box)
        {
            int boxWeight = ProductWeight.CalcWeight(box);
            int boxHeight = BoxHeights[box.Size];
            return totalCarryingWeight > 0 && boxWeight + totalCarryingWeight > this.CarryingCapacity
                || totalCarryingHeight > 0 && boxHeight + totalCarryingHeight > this.CarryingMaxHeight;
        }

        private bool IsBoxOvercapacity(Box box)
        {
            int boxWeight = ProductWeight.CalcWeight(box);
            int boxHeight = BoxHeights[box.Size];
            this.LogSimple($"{box.ToBoxInfo()} weight={boxWeight} + {this.totalCarryingWeight} <= {this.CarryingCapacity}, height={boxHeight} + {this.totalCarryingHeight} <= {this.CarryingMaxHeight}");
            return this.totalCarryingWeight > 0 && boxWeight + this.totalCarryingWeight > this.CarryingCapacity
                || this.totalCarryingHeight > 0 && boxHeight + this.totalCarryingHeight > this.CarryingMaxHeight;
        }

        private void AddCarryingBox(Box box)
        {
            this.totalCarryingWeight += ProductWeight.CalcWeight(box);
            this.totalCarryingHeight += BoxHeights[box.Size];
        }

        private void AddCarryingBox(BoxData box)
        {
            this.totalCarryingWeight += ProductWeight.CalcWeight(box);
            this.totalCarryingHeight += BoxHeights[box.Size];
        }

        private void UpdateCarryingWeightAndHeight()
        {
            this.totalCarryingWeight = 0;
            this.totalCarryingHeight = 0;
            this.inventory.Boxes.ForEach(this.AddCarryingBox);
            this.LogSimple($"carrying {this.totalCarryingWeight / 1000:0.#}kg");
        }

        public void LogSimple(string msg = null)
        {
            Plugin.LogDebug($"Restocker[{this.skill.Id}] {msg}");
        }

        public void LogStat(string msg = null)
        {
            var call = msg != null ? $"{msg} " : ""; 
            Plugin.LogDebug($"Restocker[{this.skill.Id}] {call}carryingBox={this.Box}, boxCount={this.inventory.Count}");
            if (Plugin.Instance.Settings.RestockerLog)
            {
                Plugin.LogDebug($"Restocker[{this.skill.Id}] {this.inventory.Boxes.ToBoxStackInfo()}");
            }
        }

        internal void AfterFreeTargetDisplaySlot()
        {
            this.occupiedDisplaySlots
                    .Where(s => s.IsOccupiedByMe(this.restocker))
                    .ForEach(s => s.OccupiedRestocker = null);
            this.occupiedDisplaySlots.Clear();
        }

        public void SetEmptyBox()
        {
            this.Box = this.inventory.Boxes.FirstOrDefault(b => !b.HasProducts);
        }

        public class Inventory : List<InventorySlot>
        {
            public IEnumerable<Box> Boxes => this.Select(s => s.Box);

            public void Add(Box box)
            {
                if (box == null) return;
                this.Add(new InventorySlot(box, box.gameObject.layer));
            }

            public void Remove(Box box)
            {
                this.Remove(this.Find(s => s.Box.Equals(box)));
            }

            public IEnumerable<int> ProductIds => this.Where(s => s.Box.HasProducts).Select(s => s.Box.Data.ProductID).Distinct();

            public bool Contains(Box box)
            {
                return this.Any(s => s.Box.Equals(box));
            }

            public int BoxLayer(Box box)
            {
                return this.FirstOrDefault(s => s.Box.Equals(box))?.Layer ?? -1;
            }
        }

        public class InventorySlot
        {
            public Box Box;
            public int Layer;

            public InventorySlot(Box box, int layer)
            {
                this.Box = box;
                this.Layer = layer;
            }
        }

    }
}