using System;
using Microsoft.Practices.Unity;

namespace Unity.LoggingExtension
{
	class Program
	{
		static void Main(string[] args)
		{
			var unityContainer = new UnityContainer();
			unityContainer.AddNewExtension<LoggingExtension>();
			unityContainer.Configure<ILoggingExtensionConfigurator>()
				.FormatType<IEvent>(e => string.Format("[{0}]", e.Id));

			unityContainer.RegisterType<ILogger, Logger>();
			unityContainer.RegisterType<FilePersistence>("File");
			unityContainer.RegisterType<IPersistence, DbPersistence>("Db");
			unityContainer.RegisterType<IPersistence, MongoPersistence>("Mongo");
			
			var persistence1 = unityContainer.Resolve<FilePersistence>("File");
			persistence1.Store(date: DateTime.Now, @event: new Event(2));

			var persistence2 = unityContainer.Resolve<IPersistence>("Db");
			persistence2.Store(new Event(1), DateTime.Now);
		}
	}

	public interface ILogger
	{
		void Log(string message);
	}

	public class Logger : ILogger
	{
		public void Log(string message)
		{
			Console.WriteLine(message);
		}
	}

	[Log]
	public interface IPersistence
	{
		IEvent Store(IEvent @event, DateTime? date = null);
	}

    [Log]
	public class FilePersistence
	{
        public virtual IEvent Store(IEvent @event, DateTime? date = null)
		{
			Console.WriteLine("File Store");

			return @event;
		}
	}

	public class DbPersistence : IPersistence
	{
        public IEvent Store(IEvent @event, DateTime? date = null)
		{
			Console.WriteLine("Db Store");
			return @event;
		}
	}

	public class MongoPersistence : IPersistence
	{
		public IEvent Store(IEvent @event, DateTime? date = null)
		{
			return @event;
		}
	}

    public interface IEvent
	{
		long Id { get; set; }
	}

	public class Event : IEvent
	{
		public Event(long id)
		{
			Id = id;
		}

		public long Id { get; set; }
	}
}
