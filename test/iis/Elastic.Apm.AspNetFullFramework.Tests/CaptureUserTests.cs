// Licensed to Elasticsearch B.V under
// one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information

using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AspNetFullFrameworkSampleApp.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Elastic.Apm.AspNetFullFramework.Tests
{
	[Collection(Consts.AspNetFullFrameworkTestsCollection)]
	public class CaptureUserTests : TestsBase
	{
		public CaptureUserTests(ITestOutputHelper xUnitOutputHelper) : base(xUnitOutputHelper) { }

		[AspNetFullFrameworkFact]
		public async Task User_Should_Contain_Id_And_Email_When_Using_Authenticated_ClaimsPrincipal()
		{
			var config = Configuration.Default
				.WithDefaultLoader()
				.WithDefaultCookies();

			var context = BrowsingContext.New(config);

			// register a user
			var registerUri = Consts.SampleApp.CreateUrl("/Account/Register");
			var document = await context.OpenAsync(registerUri);
			var form = document.QuerySelector<IHtmlFormElement>("#registerForm");
			var email = "russ@example.com";
			var password = "supersecret";
			await form.SubmitAsync(new RegisterViewModel
			{
				Email = email,
				Password = password,
				ConfirmPassword = password
			});

			// now login
			var loginUri = Consts.SampleApp.CreateUrl("/Account/Login");
			document = await context.OpenAsync(loginUri);
			form = document.QuerySelector<IHtmlFormElement>("#loginForm");
			await form.SubmitAsync(new LoginViewModel
			{
				Email = email,
				Password = password,
			});

			// get the home page with the logged in user
			await context.OpenAsync(Consts.SampleApp.CreateUrl("/"));

			// verify that the user id and email are captured.
			await WaitAndCustomVerifyReceivedData(received =>
			{
				received.Transactions.Count.Should().BeGreaterThan(0);

				var transaction = received.Transactions
					.OrderByDescending(t => t.Timestamp)
					.First();

				transaction.Context.User.Should().NotBeNull();
				transaction.Context.User.UserName.Should().Be(email);
				transaction.Context.User.Email.Should().Be(email);

				// id is autogenerated in SQLite so we don't know what it'll be, only that it's populated
				transaction.Context.User.Id.Should().NotBeNullOrEmpty();
			});
		}
	}
}
