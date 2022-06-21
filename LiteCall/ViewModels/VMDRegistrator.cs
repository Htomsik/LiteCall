using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.Windows.Media.Capture;
using LiteCall.Services;
using LiteCall.Services.Authorisation;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Pages;
using LiteCall.ViewModels.ServerPages;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.ViewModels
{
    internal static class VMDRegistrator
    {
      public  static IServiceCollection RegisterVMD(this IServiceCollection services)
      {


            services.AddSingleton<INavigationService>(s => CreateMainPageNavigationServices(s));

            #region Регистрация/Авторизация/Восстановление пароля на мейн сервере

            services.AddTransient<AuthorisationPageVMD>(s => new AuthorisationPageVMD(
                CreateRegistrationPageNavigationServices(s),CreatePasswordRecoveryPageNavigationService(s), CreateMainAuthorisationServices(s),s.GetRequiredService<IEncryptServices>()));


            services.AddTransient<RegistrationPageVMD>(s => new RegistrationPageVMD(
                CreateAutPageNavigationServices(s),
                CreateMainRegistrationSevices(s),
                s.GetRequiredService<IStatusServices>(),
                CreateMainServeCaptchaServices(s),CreateMainServerGetPasswordRecoveryQuestions(s),s.GetRequiredService<IEncryptServices>()));

            services.AddTransient<PasswordRecoveryVMD>(
                s=> new PasswordRecoveryVMD(
                    CreateAutPageNavigationServices(s),
                    s.GetRequiredService<IStatusServices>(),
                    CreateMainServerGetPasswordRecoveryQuestions(s),
                    CreateMainRecoveryPasswordServices(s), s.GetRequiredService<IEncryptServices>()));

            #endregion

            #region Регистрация/Авторизация на API сервере чата

            services.AddTransient<ServerRegistrationModalVMD>(s => new ServerRegistrationModalVMD(
                CreateModalAuthorisationPageNavigationService(s),
                CreateApiRegistrationSevices(s),
                s.GetRequiredService<IStatusServices>(),
                CreateApiServeCaptchaServices(s),CreateApiServerGetPasswordRecoveryQuestions(s),s.GetRequiredService<IEncryptServices>()));


            services.AddTransient<ServerAuthorisationModalVMD>(s => new ServerAuthorisationModalVMD(
                CreateModalRegistrationPageNavigationServices(s),
                CreateModalPasswordRecoveryPageNavigationService(s),
                CreateApiAuthorisationServices(s),s.GetRequiredService<IEncryptServices>()));


            services.AddTransient<ServerPasswordRecoveryModalVMD>(
                s=> new ServerPasswordRecoveryModalVMD(
                    CreateModalAuthorisationPageNavigationService(s),
                    s.GetRequiredService<IStatusServices>(),
                    CreateApiServerGetPasswordRecoveryQuestions(s),
                    CreateApiRecoveryPasswordServices(s),s.GetRequiredService<IEncryptServices>()));


            #endregion

            #region Остальные VMD

            services.AddTransient<MainPageVMD>(
                s => new MainPageVMD(s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<ServerAccountStore>(),
                    s.GetRequiredService<ServersAccountsStore>(),
                    s.GetRequiredService<CurrentServerStore>(),
                    s.GetRequiredService<MainPageServerNavigationStore>(),
                    CreateSettingPageNavigationService(s),
                    CreateServerPageNavigationService(s),
                    CreateModalAuthorisationPageNavigationService(s),
                    CreateAuthCheckApiServerSevices(s),
                    s.GetRequiredService<IStatusServices>(),
                    s.GetRequiredService<IhttpDataServices>()));

            services.AddTransient<SettingVMD>(s => new SettingVMD(
                s.GetRequiredService<AccountStore>(),
                s.GetRequiredService<ServersAccountsStore>(),s.GetRequiredService<SettingsStore>(),
                CreateAutPageNavigationServices(s),s.GetRequiredService<IhttpDataServices>(),s.GetRequiredService<IStatusServices>(),
                s.GetRequiredService<SettingsAccNavigationStore>()
            ));


            services.AddTransient<ServerVMD>(s =>
                new ServerVMD(s.GetRequiredService<ServerAccountStore>(), s.GetRequiredService<CurrentServerStore>(),s.GetRequiredService<IStatusServices>()));



            services.AddSingleton<MainWindowVMD>(s => new MainWindowVMD(
                s.GetRequiredService<MainWindowNavigationStore>(), s.GetRequiredService<AdditionalNavigationStore>(),
                s.GetRequiredService<ModalNavigationStore>(),
                s.GetRequiredService<StatusMessageStore>(),
                s.GetRequiredService<CloseModalNavigationServices>(),
                s.GetRequiredService<CloseAdditionalNavigationServices>(),s.GetRequiredService<IStatusServices>(),CreateCloseAppSevices(s)));

            #endregion

            return services;
      }

        #region Создание навигационных сервисов для базовых VMD

        private static INavigationService CreateAutPageNavigationServices(IServiceProvider serviceProvider)
        {

            return new SettingAccNavigationServices<AuthorisationPageVMD>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<AuthorisationPageVMD>());
        }

        private static INavigationService CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new SettingAccNavigationServices<RegistrationPageVMD>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<RegistrationPageVMD>());
        }

        private static INavigationService CreatePasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
        {
            return new SettingAccNavigationServices<PasswordRecoveryVMD>
                (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                    () => serviceProvider.GetRequiredService<PasswordRecoveryVMD>());
        }

        private static INavigationService CreateMainPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new NavigationServices<MainPageVMD>(serviceProvider.GetRequiredService<MainWindowNavigationStore>(),
                () => serviceProvider.GetRequiredService<MainPageVMD>());
        }

        private static INavigationService CreateSettingPageNavigationService(IServiceProvider serviceProvider)
        {
            return new AdditionalNavigationServices<SettingVMD>(serviceProvider.GetRequiredService<AdditionalNavigationStore>(), () => serviceProvider.GetRequiredService<SettingVMD>());
        }

        private static INavigationService CreateServerPageNavigationService(IServiceProvider serviceProvider)
        {
            return new MainPageServerNavigationSevices<ServerVMD>(serviceProvider.GetRequiredService<MainPageServerNavigationStore>(), () => serviceProvider.GetRequiredService<ServerVMD>());
        }

        #endregion

        #region Модальные окна VMD

        private static INavigationService CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerRegistrationModalVMD>
            (serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<ServerRegistrationModalVMD>());
        }

        private static INavigationService CreateModalAuthorisationPageNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerAuthorisationModalVMD>(serviceProvider.GetRequiredService<ModalNavigationStore>(), () => serviceProvider.GetRequiredService<ServerAuthorisationModalVMD>());
        }

        private static INavigationService CreateModalPasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerPasswordRecoveryModalVMD>(serviceProvider.GetRequiredService<ModalNavigationStore>(), () => serviceProvider.GetRequiredService<ServerPasswordRecoveryModalVMD>());
        }

        #endregion

        #region Авторизация/Регистрация/восстановление пароля сервисы
        private static IRegistrationSevices CreateApiRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(), 
                serviceProvider.GetRequiredService<CloseModalNavigationServices>(),
                serviceProvider.GetRequiredService<IhttpDataServices>());
        }

        private static IRegistrationSevices CreateMainRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationMainServerService(serviceProvider.GetRequiredService<AccountStore>(),serviceProvider.GetRequiredService<IhttpDataServices>());
        }

        private static IAuthorisationServices CreateApiAuthorisationServices(IServiceProvider serviceProvider)
        {
            return new AuthorisationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(),
                serviceProvider.GetRequiredService<CloseModalNavigationServices>(),
                serviceProvider.GetRequiredService<IhttpDataServices>());
        }

       private static IAuthorisationServices CreateMainAuthorisationServices(IServiceProvider serviceProvider)
       {
            return new AuthoisationMainServerServices(serviceProvider.GetRequiredService<AccountStore>(),serviceProvider.GetRequiredService<IhttpDataServices>(),serviceProvider.GetRequiredService<IStatusServices>());
       }
      
        private static IAuthorisationServices CreateAuthCheckApiServerSevices(IServiceProvider serviceProvider)
        {
            return new AuthCheckApiServerSevices(serviceProvider.GetRequiredService<ServerAccountStore>(),serviceProvider.GetRequiredService<IhttpDataServices>());
        }

        private static ICaptchaServices CreateMainServeCaptchaServices(IServiceProvider serviceProvider)
        {
            return new MainServerCaptchaServices(serviceProvider.GetRequiredService<IhttpDataServices>(),
                serviceProvider.GetRequiredService<IimageServices>());
        }

        private static ICaptchaServices CreateApiServeCaptchaServices(IServiceProvider serviceProvider)
        {
            return new ApiServerCaptchaServices(serviceProvider.GetRequiredService<IhttpDataServices>(),
                serviceProvider.GetRequiredService<IimageServices>(),serviceProvider.GetRequiredService<CurrentServerStore>());
        }

        private static IGetPassRecoveryQuestionsServices CreateMainServerGetPasswordRecoveryQuestions(IServiceProvider serviceProvider)
        {
            return new MainServerGetPassRecQestionsServices(serviceProvider.GetRequiredService<IhttpDataServices>());
        }

        private static IGetPassRecoveryQuestionsServices CreateApiServerGetPasswordRecoveryQuestions(IServiceProvider serviceProvider)
        {
            return new ApiServerGetPassRecQestionsServices(serviceProvider.GetRequiredService<IhttpDataServices>(),serviceProvider.GetRequiredService<CurrentServerStore>());
        }


        private static IRecoveryPasswordServices CreateMainRecoveryPasswordServices(IServiceProvider serviceProvider)
        {
            return new MainServerRecoveryPasswordServices(serviceProvider.GetRequiredService<IhttpDataServices>());
        }

        private static IRecoveryPasswordServices CreateApiRecoveryPasswordServices(IServiceProvider serviceProvider)
        {
            return new ApiServerRecoveryPasswordServices(serviceProvider.GetRequiredService<IhttpDataServices>(),serviceProvider.GetRequiredService<CurrentServerStore>());
        }

        #endregion

        private static  ICloseAppSevices CreateCloseAppSevices(IServiceProvider serviceProvider)
        {
            return new CloseAppSevices(serviceProvider.GetRequiredService<IhttpDataServices>(),serviceProvider.GetRequiredService<AccountStore>(),serviceProvider.GetRequiredService<ServersAccountsStore>());
        }
    }
}
