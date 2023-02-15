using System;
using HECSFramework.Core;
using Strategies;

namespace Components
{
    [Serializable][Documentation(Doc.Test, "we use this component on tests purpose")]
    public sealed class TestComponent : ModifiableFloatCounterComponent, IAfterEntityInit, IWorldSingleComponent
    {
        public Strategy Strategy;

        public override int Id => 10;
        public override float SetupValue => 11;

        public int InitCount = 0;

        public void AfterEntityInit()
        {
            InitCount++;
        }
    }
}