using System.Collections;
using DevTools.Testing;
using DevTools.Testing.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonersAreNotSwines.Tests;

[TestFixture(TestType.Playing)]
public class CleaningTest
{
    [SetUp]
    public void Setup()
    {
        var builder = PawnKindDefOf.Colonist
            .AsBuilder()
            .WithName("Gerald")
            .WithFaction(Faction.OfAncientsHostile);

        var prisoner = builder.Spawn();
        prisoner.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);

        var prisonWalls = GenRadial.RadialCellsAround(Find.CurrentMap.Center, 10f, 11f);

        foreach (var prisonWall in prisonWalls)
        {
            var wall = ThingMaker.MakeThing(ThingDefOf.Wall,GenStuff.DefaultStuffFor(ThingDefOf.Wall));
            wall.SetFaction(Faction.OfPlayer);
            GenSpawn.Spawn(wall, prisonWall, Find.CurrentMap );
        }
        
        var bed = (Building_Bed)ThingMaker.MakeThing(ThingDefOf.Bed,GenStuff.DefaultStuffFor(ThingDefOf.Bed));
        bed.SetFaction(Faction.OfPlayer);
        GenSpawn.Spawn(bed, prisoner.Position + new IntVec3(3,0,0), prisoner.Map);
        
        bed.ForOwnerType = BedOwnerType.Prisoner;
        bed.GetRoom().Notify_RoomShapeChanged();
        prisoner.ownership.ClaimBedIfNonMedical(bed);

        Tweaker.SuperFast = true;
    }

    [Test]
    public IEnumerator Test1()
    {
        var filthList = new List<Thing>();
        
        var filthCells = GenRadial.RadialCellsAround(Find.CurrentMap.Center, 0f, 5f);

        foreach (var filthCell in filthCells)
        {
            var filth = ThingMaker.MakeThing(ThingDefOf.Filth_Blood);
            GenSpawn.Spawn(filth, filthCell, Find.CurrentMap);
            filthList.Add(filth);
            Find.CurrentMap.areaManager.Home[filthCell] = true;
        }
        
        yield return new WaitUntilTimeout(() => filthList.All(filth => !filth.Spawned));
        
    }
    
    [TearDown]
    public void TearDown()
    {
        Tweaker.SuperFast = false;
    }
}