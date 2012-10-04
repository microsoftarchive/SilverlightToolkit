// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides organized animations for the hub tiles.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public static class HubTileService
    {
        /// <summary>
        /// Number of steps in the pipeline
        /// </summary>
        private const int WaitingPipelineSteps = 3;

        /// <summary>
        /// Number of hub tile that can be animated at exactly the same time.
        /// </summary>
        private const int NumberOfSimultaneousAnimations = 1;

        /// <summary>
        /// Track resurrection for weak references.
        /// </summary>
        private const bool TrackResurrection = false;

        /// <summary>
        /// Timer to trigger animations in timely.
        /// </summary>        
        private static DispatcherTimer Timer = new DispatcherTimer();

        /// <summary>
        /// Random number generator to take certain random decisions.
        /// e.g. which hub tile is to be animated next.
        /// </summary>
        private static Random ProbabilisticBehaviorSelector = new Random();

        /// <summary>
        /// Pool that contains references to the hub tiles that are not frozen.
        /// i.e. hub tiles that can be animated at the moment.
        /// </summary>
        private static List<WeakReference> EnabledImagesPool = new List<WeakReference>();

        /// <summary>
        /// Pool that contains references to the hub tiles which are frozen.
        /// i.e. hub tiles that cannot be animated at the moment.
        /// </summary>
        private static List<WeakReference> FrozenImagesPool = new List<WeakReference>();

        /// <summary>
        /// Pipeline that contains references to the hub tiles that where animated previously.
        /// These are stalled briefly before they can be animated again.
        /// </summary>
        private static List<WeakReference> StalledImagesPipeline = new List<WeakReference>();

        /// <summary>
        /// Static constructor to add the tick event handler.
        /// </summary>        
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Attaching event handlers cannot be done inline.")]
        static HubTileService()
        {
            Timer.Tick += OnTimerTick;
        }

        /// <summary>
        /// Restart the timer to trigger animations.
        /// </summary>
        private static void RestartTimer()
        {
            if (!Timer.IsEnabled)
            {        
                Timer.Interval = TimeSpan.FromMilliseconds(2500);
                Timer.Start();
            }
        }

        /// <summary>
        /// Add a reference to a newly instantiated hub tile.
        /// </summary>
        /// <param name="tile">The newly instantiated hub tile.</param>
        internal static void InitializeReference(HubTile tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            if (tile.IsFrozen)
            {
                AddReferenceToFrozenPool(wref);  
            }
            else
            {
                AddReferenceToEnabledPool(wref);  
            }

            RestartTimer();
        }

        /// <summary>
        /// Remove all references of a hub tile before finalizing it.
        /// </summary>
        /// <param name="tile">The hub tile that is to be finalized.</param>
        internal static void FinalizeReference(HubTile tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            HubTileService.RemoveReferenceFromEnabledPool(wref);
            HubTileService.RemoveReferenceFromFrozenPool(wref);
            HubTileService.RemoveReferenceFromStalledPipeline(wref);
        }

        /// <summary>
        /// Add a reference of a hub tile to the enabled images pool.
        /// </summary>
        /// <param name="tile">The hub tile to be added.</param>
        private static void AddReferenceToEnabledPool(WeakReference tile)
        {
            if (!ContainsTarget(EnabledImagesPool, tile.Target))
            {
                EnabledImagesPool.Add(tile);
            }
        }

        /// <summary>
        /// Add a reference of a hub tile to the frozen images pool.
        /// </summary>
        /// <param name="tile">The hub tile to be added.</param>
        private static void AddReferenceToFrozenPool(WeakReference tile)
        {
            if (!ContainsTarget(FrozenImagesPool, tile.Target))
            {
                FrozenImagesPool.Add(tile);
            }
        }

        /// <summary>
        /// Add a reference of a hub tile to the stalled images pipeline.
        /// </summary>
        /// <param name="tile">The hub tile to be added.</param>
        private static void AddReferenceToStalledPipeline(WeakReference tile)
        {
            if (!ContainsTarget(StalledImagesPipeline, tile.Target))
            {
                StalledImagesPipeline.Add(tile);
            }
        }

        /// <summary>
        /// Remove the reference of a hub tile from the enabled images pool.
        /// </summary>
        /// <param name="tile">The hub tile to be removed.</param>
        private static void RemoveReferenceFromEnabledPool(WeakReference tile)
        {
            RemoveTarget(EnabledImagesPool, tile.Target);
        }

        /// <summary>
        /// Remove the reference of a hub tile from the frozen images pool.
        /// </summary>
        /// <param name="tile">The hub tile to be removed.</param>
        private static void RemoveReferenceFromFrozenPool(WeakReference tile)
        {
            RemoveTarget(FrozenImagesPool, tile.Target);
        }

        /// <summary>
        /// Remove the reference of a hub tile from the stalled images pipeline.
        /// </summary>
        /// <param name="tile">The hub tile to be removed.</param>
        private static void RemoveReferenceFromStalledPipeline(WeakReference tile)
        {
            RemoveTarget(StalledImagesPipeline, tile.Target);
        }

        /// <summary>
        /// Determine if there is a reference to a known target in a list.
        /// </summary>
        /// <param name="list">The list to be examined.</param>
        /// <param name="target">The known target.</param>
        /// <returns>True if a reference to the known target exists in the list. False otherwise.</returns>
        private static bool ContainsTarget(List<WeakReference> list, Object target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Target == target)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove a reference to a known target in a list.
        /// </summary>
        /// <param name="list">The list to be examined.</param>
        /// <param name="target">The known target.</param>
        private static void RemoveTarget(List<WeakReference> list, Object target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Target == target)
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the code to process a visual transition:
        /// 1. Stop the timer.
        /// 2. Advances the stalled tiles to the next step in the pipeline.
        /// If there is at least one tile that can be currently animated ...
        /// 3. Animate as many tiles as indicated.
        /// 4. Select a tile andomly from the pool of enabled tiles.
        /// 5. Based on this tile's current visual state, move it onto 
        /// the next one.
        /// 6. Set the stalling counter for the recently animated image.
        /// 7. Take it out from the pool and into the pipeline to prevent it 
        /// from being animated continuously.
        /// 8. Restart the timer with a randomly generated time interval
        /// between 100 and 3000 ms.
        /// Notice that if there are no hub tiles that can be animated, 
        /// the timer is not restarted.
        /// </summary>
        /// <param name="sender">The static timer.</param>
        /// <param name="e">The event information.</param>
        private static void OnTimerTick(object sender, EventArgs e)
        {
            Timer.Stop();

            for (int i = 0; i < StalledImagesPipeline.Count; i++)
            {
                if ((StalledImagesPipeline[i].Target as HubTile)._stallingCounter-- == 0)
                {
                    AddReferenceToEnabledPool(StalledImagesPipeline[i]);
                    RemoveReferenceFromStalledPipeline(StalledImagesPipeline[i]);
                    i--;
                }
            }

            if (EnabledImagesPool.Count > 0) 
            {
                for (int j = 0; j < NumberOfSimultaneousAnimations; j++)
                {
                    int index = ProbabilisticBehaviorSelector.Next(EnabledImagesPool.Count);

                    switch ((EnabledImagesPool[index].Target as HubTile).State)
                    {
                        case ImageState.Expanded:
                            //If the tile can neither drop nor flip, or if its size is Small, do not change state.
                            if ((!(EnabledImagesPool[index].Target as HubTile)._canDrop && !(EnabledImagesPool[index].Target as HubTile)._canFlip) || (EnabledImagesPool[index].Target as HubTile).Size == TileSize.Small)
                            {
                                break;
                            }

                            //If the tile can only flip, change to the Flipped state.
                            if (!(EnabledImagesPool[index].Target as HubTile)._canDrop && (EnabledImagesPool[index].Target as HubTile)._canFlip)
                            {
                                (EnabledImagesPool[index].Target as HubTile).State = ImageState.Flipped;
                                break;
                            }

                            //If the tile can only drop, change to the Semidropped state.
                            if (!(EnabledImagesPool[index].Target as HubTile)._canFlip && (EnabledImagesPool[index].Target as HubTile)._canDrop)
                            {
                                (EnabledImagesPool[index].Target as HubTile).State = ImageState.Semiexpanded;
                                break;
                            }

                            //If the tile can drop and flip, change randomly either to the Semidropped state or the Flipped state.
                            if (ProbabilisticBehaviorSelector.Next(2) == 0)
                            {
                                (EnabledImagesPool[index].Target as HubTile).State = ImageState.Semiexpanded;
                            }
                            else
                            {
                                (EnabledImagesPool[index].Target as HubTile).State = ImageState.Flipped;
                            }
                            break;
                        case ImageState.Semiexpanded:
                            (EnabledImagesPool[index].Target as HubTile).State = ImageState.Collapsed;
                            break;
                        case ImageState.Collapsed:
                            (EnabledImagesPool[index].Target as HubTile).State = ImageState.Expanded;
                            break;
                        case ImageState.Flipped:
                            (EnabledImagesPool[index].Target as HubTile).State = ImageState.Expanded;
                            break;
                    }
                    (EnabledImagesPool[index].Target as HubTile)._stallingCounter = WaitingPipelineSteps;
                    AddReferenceToStalledPipeline(EnabledImagesPool[index]);
                    RemoveReferenceFromEnabledPool(EnabledImagesPool[index]);
                }
            }
            else if (StalledImagesPipeline.Count == 0)
            {
                return;
            }

            Timer.Interval = TimeSpan.FromMilliseconds(ProbabilisticBehaviorSelector.Next(1, 31) * 100);
            Timer.Start();
        }

        /// <summary>
        /// Freeze a hub tile.
        /// </summary>
        /// <param name="tile">The hub tile to be frozen.</param>
        public static void FreezeHubTile(HubTile tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            AddReferenceToFrozenPool(wref);
            RemoveReferenceFromEnabledPool(wref);
            RemoveReferenceFromStalledPipeline(wref);
        }

        /// <summary>
        /// Unfreezes a hub tile and restarts the timer if needed.
        /// </summary>
        /// <param name="tile">The hub tile to be unfrozen.</param>
        public static void UnfreezeHubTile(HubTile tile)
        {
            WeakReference wref = new WeakReference(tile, TrackResurrection);
            AddReferenceToEnabledPool(wref);
            RemoveReferenceFromFrozenPool(wref);
            RemoveReferenceFromStalledPipeline(wref);

            RestartTimer();
        }

        /// <summary>
        /// Freezes all the hub tiles with the specified group tag that are not already frozen.
        /// </summary>
        /// <param name="group">The group tag representing the hub tiles that should be frozen.</param>
        public static void FreezeGroup(string group)
        {
            for (int i = 0; i < EnabledImagesPool.Count; i++)
            {
                if ((EnabledImagesPool[i].Target as HubTile).GroupTag == group)
                {
                    (EnabledImagesPool[i].Target as HubTile).IsFrozen = true;
                    i--;
                }
            }

            for (int j = 0; j < StalledImagesPipeline.Count; j++)
            {
                if ((StalledImagesPipeline[j].Target as HubTile).GroupTag == group)
                {
                    (StalledImagesPipeline[j].Target as HubTile).IsFrozen = true;
                    j--;
                }
            }
        }

        /// <summary>
        /// Unfreezes all the hub tiles with the specified group tag 
        /// that are currently frozen and restarts the timer if needed.
        /// </summary>
        /// <param name="group">The group tag representing the hub tiles that should be unfrozen.</param>
        public static void UnfreezeGroup(string group)
        {
            for (int i = 0; i < FrozenImagesPool.Count; i++)
            {
                if ((FrozenImagesPool[i].Target as HubTile).GroupTag == group)
                {
                    (FrozenImagesPool[i].Target as HubTile).IsFrozen = false;
                    i--;
                }
            }

            RestartTimer();
        }
    }
}