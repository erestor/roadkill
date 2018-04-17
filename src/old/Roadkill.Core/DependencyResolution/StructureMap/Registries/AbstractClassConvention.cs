using System;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using WebGrease.Css.Extensions;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
    public class AbstractClassConvention<T> : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed).ForEach(type =>
            {
                if (TypeExtensions.CanBeCastTo<T>((Type) type))
                {
                    registry.For(typeof(T)).LifecycleIs(new UniquePerRequestLifecycle()).Add((Type) type);
                }
            });
        }
    }
}