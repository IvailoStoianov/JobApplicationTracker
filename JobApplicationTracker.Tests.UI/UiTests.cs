using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace JobApplicationTracker.Tests.UI
{
    public class UiTests : PageTest
    {
        private static string BaseUrl = "http://localhost:5173";
        private static string ApiBaseUrl = "http://localhost:5088";

        private async Task<(string Email, string Password)> RegisterViaApi()
        {
            var email = $"test_{Guid.NewGuid():N}@example.com";
            var password = "P@ssw0rd!1";

            var api = await Playwright.APIRequest.NewContextAsync(new() { BaseURL = ApiBaseUrl, IgnoreHTTPSErrors = true });
            var username = $"user_{Guid.NewGuid():N}".Substring(0, 16);
            var res = await api.PostAsync("/api/Authentication/Register", new() { DataObject = new { username, email, password, confirmPassword = password } });
            if (!res.Ok)
            {
                var body = await res.TextAsync();
                throw new Exception($"Register failed: {res.Status} {body}");
            }
            return (email, password);
        }

        private async Task<string> GetTokenAsync(string email, string password)
        {
            var api = await Playwright.APIRequest.NewContextAsync(new() { BaseURL = ApiBaseUrl, IgnoreHTTPSErrors = true });
            var res = await api.PostAsync("/api/Authentication/Login", new() { DataObject = new { email, password } });
            if (!res.Ok)
            {
                var body = await res.TextAsync();
                throw new Exception($"Login failed: {res.Status} {body}");
            }
            using var doc = JsonDocument.Parse(await res.TextAsync());
            var token = doc.RootElement.GetProperty("data").GetProperty("accessToken").GetString();
            return token!;
        }

        [Test]
        public async Task Smoke_Register_Login_SeeJobs()
        {
            var creds = await RegisterViaApi();
            var token = await GetTokenAsync(creds.Email, creds.Password);
            await Page.AddInitScriptAsync($"localStorage.setItem('jat.token','{token}'); localStorage.setItem('jat.email','{creds.Email}');");
            await Page.GotoAsync(BaseUrl + "/");
            await Expect(Page.GetByText("Job Application Tracker")).ToBeVisibleAsync();
            await Expect(Page.GetByTestId("add-job").Or(Page.GetByRole(AriaRole.Button, new() { Name = "Add Job" }))).ToBeVisibleAsync();
        }

        [Test]
        public async Task Jobs_CRUD_Flow()
        {
            var creds = await RegisterViaApi();
            var token = await GetTokenAsync(creds.Email, creds.Password);
            await Page.AddInitScriptAsync($"localStorage.setItem('jat.token','{token}'); localStorage.setItem('jat.email','{creds.Email}');");
            await Page.GotoAsync(BaseUrl + "/");
            await Expect(Page.GetByText("Job Application Tracker")).ToBeVisibleAsync();

            // Create via API using token, then refresh UI
            var api = await Playwright.APIRequest.NewContextAsync(new() { BaseURL = ApiBaseUrl, IgnoreHTTPSErrors = true, ExtraHTTPHeaders = new System.Collections.Generic.Dictionary<string, string> { ["Authorization"] = $"Bearer {token}" } });
            var createRes = await api.PostAsync("/api/Jobs/CreateJob", new() { DataObject = new { company = "Playwright Co", position = "QA Engineer", status = "Applied", applicationDate = DateTime.UtcNow.ToString("o"), notes = "", salary = 120000, contact = "qa@playwright.dev" } });
            if (!createRes.Ok)
            {
                var body = await createRes.TextAsync();
                throw new Exception($"Create job failed: {createRes.Status} {body}");
            }
            await Page.ReloadAsync();
            await Expect(Page.GetByTestId("jobs-table").Or(Page.Locator("table"))).ToContainTextAsync("Playwright Co", new() { Timeout = 20000 });

            // Edit
            var row = Page.Locator("tr:has-text(\"Playwright Co\")").First;
            await row.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
            var position = Page.GetByPlaceholder("Position");
            await position.FillAsync("");
            await position.FillAsync("Senior QA Engineer");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
            await Expect(Page.GetByText("Senior QA Engineer")).ToBeVisibleAsync();

            // View notes (modal should be above)
            await row.GetByRole(AriaRole.Button, new() { Name = "View Notes" }).ClickAsync();
            await Expect(Page.GetByText("Notes - Playwright Co")).ToBeVisibleAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Close" }).ClickAsync();

            // Delete (accept confirm)
            Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
            await row.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
            await Expect(Page.GetByText("Playwright Co")).ToBeHiddenAsync();
        }
    }
}


