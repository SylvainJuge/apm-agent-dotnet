// Licensed to Elasticsearch B.V under
// one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System;
using System.Runtime.InteropServices;

namespace Elastic.Apm.Tests.Utilities
{
	public static class TestEnvironment
	{
		public static bool IsCi { get; }

		public static bool IsLinux { get; }

		public static bool IsWindows { get; }

		static TestEnvironment()
		{
			// TODO this IsCi check is no longer valid, once we are green on Github Action
			// we should be able to remove all special handling related to this
			IsCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BUILD_ID"));
			IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		}
	}
}
