using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace CarSample
{
    public interface ICarContract
    {
        string DoSomething();
    }

    [Export(typeof(ICarContract))]
    public class CarBMW : ICarContract
    {
        [Import]
        public CarGarage CarGarage { get; set; }

        public CarBMW()
        {
            Console.WriteLine("Constructor CarBMW: " + this.GetHashCode());
        }
        public string DoSomething()
        {
            return "CarBMW " + this.GetHashCode() + " Garage: " + CarGarage.GetHashCode();
        }
    }

    [Export(typeof(ICarContract))]
    public class CarMercedes : ICarContract
    {
        [Import]
        public CarGarage CarGarage { get; set; }

        public CarMercedes()
        {
            Console.WriteLine("Constructor CarMercedes: " + this.GetHashCode());
        }
        public string DoSomething()
        {
            return "CarMercedes " + this.GetHashCode() + " Garage: " + CarGarage.GetHashCode();
        }
    }

    [Export]
    public class CarGarage
    {
        public CarGarage()
        {
            Console.WriteLine("Constructor CarGarage: " + this.GetHashCode());
        }
    }

    [Export]
    public class CarHost
    {
        [ImportMany]
        public ICarContract[] CarParts { get; set; }
    }

    [Export]
    public class CarManager
    {
        [Import]
        private ExportFactory<CarHost> carHostFactory = null;

        public ExportLifetimeContext<CarHost> CreateCarHost()
        {
            ExportLifetimeContext<CarHost> carHostContext = carHostFactory.CreateExport();
            return carHostContext;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }
        void Run()
        {
            // var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            // var container = new CompositionContainer(catalog);

            var managerCatalog = new TypeCatalog(typeof(CarManager));
            var partCatalog = new TypeCatalog(typeof(CarHost), typeof(CarMercedes), typeof(CarBMW), typeof(CarGarage));

            var scope = new CompositionScopeDefinition(
                        managerCatalog,
                        new[] { new CompositionScopeDefinition(partCatalog, null) });

            var container = new CompositionContainer(scope);

            var carManager = container.GetExportedValue<CarManager>();

            ExportLifetimeContext<CarHost> carHostContextA = carManager.CreateCarHost();
            Console.WriteLine("");

            ExportLifetimeContext<CarHost> carHostContextB = carManager.CreateCarHost();
            Console.WriteLine("");

            foreach (ICarContract carParts in carHostContextA.Value.CarParts)
                Console.WriteLine(carParts.DoSomething());
            carHostContextA.Dispose();
            Console.WriteLine("");

            foreach (ICarContract carParts in carHostContextB.Value.CarParts)
                Console.WriteLine(carParts.DoSomething());
            carHostContextB.Dispose();
            Console.WriteLine("");

            Console.ReadLine();
        }
    }
}
