using EmployManager.Models;
using EmployManager.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EmployManager.Services
{
    public class Rest 
    {
        static string Host = @"http://192.168.31.207:5224/";
        /// <summary>
        ///  private static  string Host= @"http://192.168.31.205:5224/";
        /// </summary>

        private static string ServiceName = "Vendor";
        private static HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-api-key", ServiceName);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {BaseViewModel.Token}");

            /// client.DefaultRequestHeaders.Add("app-token", $"{}");

            return client;
        }

        public static async Task<string> Login(string handle, string password)
        {
           
            var client = GetClient();
            var json_content = new { handle = handle, password = password,};
            var json = JsonConvert.SerializeObject(json_content);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{Host}api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var temp = await response.Content.ReadAsStringAsync();
                    return temp.Replace("\"", "");
                }
                else
                    throw new Exception(await response.Content.ReadAsStringAsync());

            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }


        }


        public static async Task<Organization> GetOrganization()
        {
            var client = GetClient();
            

            try
            {
                var response = await client.GetAsync($"{Host}api/auth/login");

                if (response.IsSuccessStatusCode)
                {
                    var json_content = await response.Content.ReadAsStringAsync();
                    var organization = JsonConvert.DeserializeObject<Organization>(json_content);
                    return organization;
                }
                else
                    throw new Exception(await response.Content.ReadAsStringAsync());

            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }

        }
    }
}
