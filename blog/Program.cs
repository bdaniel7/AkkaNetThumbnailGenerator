using System;
using System.Diagnostics;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AkkaNetThumbnailGenerator
{
	static class Program
	{
		static void Main(string[] args)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var config = ConfigurationFactory.ParseString(File.ReadAllText("akkaconfig.hocon"));

			using (ActorSystem system = ActorSystem.Create("ThumbnailGenerator", config))
			{
				var postPaths = Directory.GetFiles(Constants.PostsDirectory);

				var yamlDeserializer = new DeserializerBuilder()
				                  .WithNamingConvention(CamelCaseNamingConvention.Instance)
				                  .IgnoreUnmatchedProperties()
				                  .Build();

				var startReadingPosts = system.ActorOf(Props.Create<ReadingPostsCoordinatorActor>(yamlDeserializer, postPaths),
																											"startReadingPostsActor");

				startReadingPosts.Tell(new ReadingPostsCoordinatorActor.StartReadingPosts());

				Console.ReadKey(true);
				system.Terminate();

				stopwatch.Stop();
				Console.WriteLine("Completed in {0}", stopwatch.Elapsed);

				Console.WriteLine("Stopped. Press any key to exit.");
				Console.ReadKey(true);

			}
		}
	}
}