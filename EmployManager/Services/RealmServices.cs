
using EmployManager.Helpers;
using Realms;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployManager.Services
{
    public static class RealmService
    {
        private static bool serviceInitialised;

        private static Realms.Sync.App app;

        private static Realm mainThreadRealm;

        public static Realms.Sync.User CurrentUser => app.CurrentUser;

        public static string CurrentLogin { get => Preferences.Get(nameof(CurrentLogin), ""); set => Preferences.Set(nameof(CurrentLogin), value); }
        public static string CurrentDepartamentId { get => Preferences.Get(nameof(CurrentDepartamentId), ""); set => Preferences.Set(nameof(CurrentDepartamentId), value); }
        public static string CurrentOrganizationId { get => Preferences.Get(nameof(CurrentOrganizationId), ""); set => Preferences.Set(nameof(CurrentOrganizationId), value); }


        public static async Task Init()
        {

            if (serviceInitialised)
            {
                return;
            }

            //using Stream fileStream = await FileSystem.AppDataDirectory.OpenAppPackageFileAsync("atlasConfig.json");
            //using StreamReader reader = new StreamReader(fileStream);
            //var fileContent = await reader.ReadToEndAsync();

            //var config = JsonSerializer.Deserialize<RealmAppConfig>(fileContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var config = new RealmAppConfig
            {
                AppId = "testapp-qcmdl",
                BaseUrl = "https://realm.mongodb.com"
            };

            var appConfiguration = new Realms.Sync.AppConfiguration(config.AppId)
            {
                BaseUri = new Uri(config.BaseUrl)
            };

            app = Realms.Sync.App.Create(appConfiguration);


            serviceInitialised = true;
            await Task.CompletedTask;
        }

        public static Realm GetMainThreadRealm()
        {

            return mainThreadRealm ??= GetRealm();
        }

        public static async Task SetSubscription(Realm realm, SubscriptionType subType)
        {


            realm.Subscriptions.Update(() =>
            {

                var (queryUser, queryNameUser) = GetQueryForSubUser(realm, SubscriptionType.All);
                var (queryDepartament, queryNameDepartament) = GetQueryForDepartaments(realm, SubscriptionType.All);
                var (queryContacts, queryNameContacts) = GetQueryContacts(realm, SubscriptionType.All);
                var (queryOrganization,queryNameOrganization)= GetQueryOrganizations(realm, SubscriptionType.All);




                realm.Subscriptions.Add(queryOrganization, new SubscriptionOptions { Name = queryNameOrganization });
                realm.Subscriptions.Add(queryUser, new SubscriptionOptions { Name = queryNameUser });
                realm.Subscriptions.Add(queryDepartament, new SubscriptionOptions { Name = queryNameDepartament });
                realm.Subscriptions.Add(queryContacts, new SubscriptionOptions { Name = queryNameContacts });



            });

            //There is no need to wait for synchronization if we are disconnected
            if (realm.SyncSession.ConnectionState != ConnectionState.Disconnected)
            {
                await realm.Subscriptions.WaitForSynchronizationAsync();
            }
        }

        public static SubscriptionType GetCurrentSubscriptionType(Realm realm)
        {
            var activeSubscription = realm.Subscriptions.FirstOrDefault();

            return activeSubscription.Name switch
            {
                "all" => SubscriptionType.All,
                "mine" => SubscriptionType.Mine,
                _ => throw new InvalidOperationException("Unknown subscription type")
            };
        }
        public static Realm GetRealm()
        {

            var config = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    var (queryUser, queryNameUser) = GetQueryForSubUser(realm, SubscriptionType.All);
                    var (queryDepartament, queryNameDepartament) = GetQueryForDepartaments(realm, SubscriptionType.All);
                    var (queryContacts, queryNameContacts) = GetQueryContacts(realm, SubscriptionType.All);
                    var (queryOrganization, queryNameOrganization) = GetQueryOrganizations(realm, SubscriptionType.All);




                    realm.Subscriptions.Add(queryOrganization, new SubscriptionOptions { Name = queryNameOrganization });
                    realm.Subscriptions.Add(queryUser, new SubscriptionOptions { Name = queryNameUser });
                    realm.Subscriptions.Add(queryDepartament, new SubscriptionOptions { Name = queryNameDepartament });
                    realm.Subscriptions.Add(queryContacts, new SubscriptionOptions { Name = queryNameContacts });

                }
            };

            return Realm.GetInstance(config);

        }
        private static (IQueryable<Models.Member> Query, string Name) GetQueryForSubUser(Realm realm, SubscriptionType subType)
        {
            IQueryable<Models.Member> query = null;
            string queryName = null;

            if (subType == SubscriptionType.All)
            {
                query = realm.All<Models.Member>();
                queryName = "allUsers";
            }
            else if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Models.Member>().Where(x => x.DepartamentId == CurrentDepartamentId);
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }      
        private static (IQueryable<Models.Organization> Query, string Name) GetQueryOrganizations(Realm realm, SubscriptionType subType)
        {
            IQueryable<Models.Organization> query = null;
            string queryName = null;

            if (subType == SubscriptionType.All)
            {
                query = realm.All<Models.Organization>();
                queryName = "allOrganizations";
            }
            else if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Models.Organization>();
                queryName = "allOrganizations";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }
        private static (IQueryable<Models.Departanent> Query, string Name) GetQueryForDepartaments(Realm realm, SubscriptionType subType)
        {
            IQueryable<Models.Departanent> query = null;
            string queryName = null;

            if (subType == SubscriptionType.All)
            {
                query = realm.All<Models.Departanent>();
                queryName = "allDepartaments";
            }else if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Models.Departanent>() ;
                queryName = "mineDepartaments";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }
        private static (IQueryable<Models.Contacts> Query, string Name) GetQueryContacts(Realm realm, SubscriptionType subType)
        {
            IQueryable<Models.Contacts> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<Models.Contacts>();
                queryName = "mineContacts";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<Models.Contacts>();
                queryName = "allContacts";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }


     
        public static async Task LoginAsync()
        {
            try
            {



                if (app == null)
                    await Init();
                var user = await app.LogInAsync(Credentials.ApiKey(Const.ApiKey));

                using var realm = GetRealm();

                await SetSubscription(realm, SubscriptionType.All);
                await realm.Subscriptions.WaitForSynchronizationAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }
        }



        public static async Task ChangePasswordAsync(string target_login)
        {
            await app.EmailPasswordAuth.SendResetPasswordEmailAsync(target_login);
        }

        public static async Task LogoutAsync()
        {
            mainThreadRealm.Subscriptions.Update(() =>
            {
                mainThreadRealm.Subscriptions.RemoveAll(true);
            });

            //serviceInitialised = false;
            await app.CurrentUser.LogOutAsync();
            mainThreadRealm?.Dispose();
            mainThreadRealm = null;
        }




        public enum SubscriptionType
        {
            Mine,
            All,
        }

        public class RealmAppConfig
        {
            public string AppId { get; set; }

            public string BaseUrl { get; set; }
        }

    }
}

