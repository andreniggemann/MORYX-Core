// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moryx.Model;
using Moryx.Model.Attributes;
using Moryx.Model.Repositories;

namespace Moryx.TestTools.Test.Model
{
    [ModelSetup(typeof(TestModelContext))]
    public class AnySetup : IModelSetup
    {
        public int SortOrder => 1;

        public string Name => "Some setup";

        public string Description => "Creates some cars";

        public string SupportedFileRegex => string.Empty;

        public async Task Execute(IUnitOfWork openContext, string setupData)
        {
            var carRepo = openContext.GetRepository<ICarEntityRepository>();
            var wheelRepo = openContext.GetRepository<IWheelEntityRepository>();

            CarEntity lastCar = null;
            for (var i = 0; i < 1; i++)
            {
                var carEntity = carRepo.Create();
                carEntity.Name = "Car " + i;
                carEntity.Price = i + 100;

                void CreateWheel(WheelType wheelType)
                {
                    var wheelEntity = wheelRepo.Create();
                    wheelEntity.WheelType = wheelType;
                    wheelEntity.Car = carEntity;
                }

                CreateWheel(WheelType.FrontLeft);
                CreateWheel(WheelType.FrontRight);
                CreateWheel(WheelType.RearLeft);
                CreateWheel(WheelType.RearRight);

                lastCar = carEntity;
            }

            await openContext.SaveChangesAsync();

            carRepo.Remove(lastCar);

            await openContext.SaveChangesAsync();

            var allCarsWithLazyWheels = carRepo.Linq.ToList();

            var allCarsWithWheels = carRepo.Linq.Include(c => c.Wheels).ToList();

            // All cars with exact name "Car 1"
            var allNamedCar1 = carRepo.Linq.Where(c => c.Name == "Car 1");

            var firstContains = carRepo.Linq.First(c => c.Name.Contains("Car"));

            var allContains = carRepo.Linq.Where(c => c.Name.Contains("Car"));
        }
    }
}
