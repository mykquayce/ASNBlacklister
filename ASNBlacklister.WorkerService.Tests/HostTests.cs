using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ASNBlacklister.WorkerService.Tests
{
	public sealed class HostTests : IClassFixture<Fixtures.HostFixture>
	{
		private readonly IHost _host;

		public HostTests(Fixtures.HostFixture hostFixture)
		{
			_host = hostFixture.Host;
		}

		[Fact]
		public void StepsAreDependencyInjected()
		{
			var types = GetStepTypes().ToList();

			Assert.NotEmpty(types);
			Assert.DoesNotContain(default, types);

			var names = types.Select(t => t.Name).ToList();

			foreach (var type in types)
			{
				var step = _host.Services.GetService(type);
				Assert.False(step is null, type.Name + " cannot be injected.");
				Assert.EndsWith("Step", type.Name, StringComparison.InvariantCultureIgnoreCase);
			}
		}

		private static IEnumerable<Type> GetStepTypes()
		{
			var stepType = typeof(WorkflowCore.Interface.IStepBody);
			var type = typeof(ASNBlacklister.Workflows.Workflow);
			var assembly = type.Assembly;
			var types = assembly.GetTypes();

			return from t in types
				   where string.Equals(t.Namespace, "ASNBlacklister.Workflows.Steps", StringComparison.InvariantCultureIgnoreCase)
				   where (t as TypeInfo)?.ImplementedInterfaces.Contains(stepType) ?? false
				   select t;
		}
	}
}
