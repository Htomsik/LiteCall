using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.Windows.Media.Capture;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.AuthRegServices.Authorization;
using LiteCall.Services.AuthRegServices.Captcha;
using LiteCall.Services.AuthRegServices.PasswordRecovery;
using LiteCall.Services.AuthRegServices.Registration;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Pages;
using LiteCall.ViewModels.ServerPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.ViewModels
{
    internal static class VMDRegistrator
    {
      public  static IServiceCollection RegisterVMD(this IServiceCollection services, IConfiguration configuration)
      {


            services.AddSingleton<INavigationService>(s => CreateMainPageNavigationServices(s));

            #region Регистрация/Авторизация/Восстановление пароля на мейн сервере

            services.AddTransient<AuthorizationPageVmd>(s => new AuthorizationPageVmd(
                CreateRegistrationPageNavigationServices(s),CreatePasswordRecoveryPageNavigationService(s), CreateMainAuthorisationServices(s),s.GetRequiredService<IEncryptServices>()));


            services.AddTransient<RegistrationPageVmd>(s => new RegistrationPageVmd(
                CreateAutPageNavigationServices(s),
                CreateMainRegistrationSevices(s),
                s.GetRequiredService<IStatusServices>(),
                CreateMainServeCaptchaServices(s),CreateMainServerGetPasswordRecoveryQuestions(s),s.GetRequiredService<IEncryptServices>()));

            services.AddTransient<PasswordRecoveryVmd>(
                s=> new PasswordRecoveryVmd(
                    CreateAutPageNavigationServices(s),
                    s.GetRequiredService<IStatusServices>(),
                    CreateMainServerGetPasswordRecoveryQuestions(s),
                    CreateMainRecoveryPasswordServices(s), s.GetRequiredService<IEncryptServices>()));

            #endregion

            #region Регистрация/Авторизация на API сервере чата

            services.AddTransient<ServerRegistrationModalVmd>(s => new ServerRegistrationModalVmd(
                CreateModalAuthorisationPageNavigationService(s),
                CreateApiRegistrationSevices(s),
                s.GetRequiredService<IStatusServices>(),
                CreateApiServeCaptchaServices(s),CreateApiServerGetPasswordRecoveryQuestions(s),s.GetRequiredService<IEncryptServices>()));


            services.AddTransient<ServerAuthorizationModalVmd>(s => new ServerAuthorizationModalVmd(
                CreateModalRegistrationPageNavigationServices(s),
                CreateModalPasswordRecoveryPageNavigationService(s),
                CreateApiAuthorisationServices(s),s.GetRequiredService<IEncryptServices>()));


            services.AddTransient<ServerPasswordRecoveryModalVmd>(
                s=> new ServerPasswordRecoveryModalVmd(
                    CreateModalAuthorisationPageNavigationService(s),
                    s.GetRequiredService<IStatusServices>(),
                    CreateApiServerGetPasswordRecoveryQuestions(s),
                    CreateApiRecoveryPasswordServices(s),s.GetRequiredService<IEncryptServices>()));


            #endregion

            #region Остальные VMD

            services.AddTransient<MainPageVmd>(
                s => new MainPageVmd(s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<ServerAccountStore>(),
                    s.GetRequiredService<SavedServersStore>(),
                    s.GetRequiredService<CurrentServerStore>(),
                    s.GetRequiredService<MainPageServerNavigationStore>(),
                    CreateSettingPageNavigationService(s),
                    CreateServerPageNavigationService(s),
                    CreateModalAuthorisationPageNavigationService(s),
                    CreateAuthCheckApiServerSevices(s),
                    s.GetRequiredService<IStatusServices>(),
                    s.GetRequiredService<IHttpDataServices>()));

            services.AddTransient<SettingVmd>(s => new SettingVmd(
                s.GetRequiredService<AccountStore>(),
                s.GetRequiredService<SavedServersStore>(),s.GetRequiredService<SettingsStore>(),
                CreateAutPageNavigationServices(s),s.GetRequiredService<IHttpDataServices>(),s.GetRequiredService<IStatusServices>(),
                s.GetRequiredService<SettingsAccNavigationStore>()
            ));


            services.AddTransient<ServerVMD>(s =>
                new ServerVMD(s.GetRequiredService<ServerAccountStore>(), s.GetRequiredService<CurrentServerStore>(),s.GetRequiredService<IStatusServices>()));



            services.AddSingleton<MainWindowVMD>(s => new MainWindowVMD(
                s.GetRequiredService<MainWindowNavigationStore>(), s.GetRequiredService<AdditionalNavigationStore>(),
                s.GetRequiredService<ModalNavigationStore>(),
                s.GetRequiredService<StatusMessageStore>(),
                s.GetRequiredService<CloseModalNavigationServices>(),
                s.GetRequiredService<CloseAdditionalNavigationServices>(),s.GetRequiredService<IStatusServices>(),s.GetRequiredService<ICloseAppServices>(), configuration));

            #endregion

            return services;
      }

        #region Создание навигационных сервисов для базовых VMD

        private static INavigationService CreateAutPageNavigationServices(IServiceProvider serviceProvider)
        {

            return new SettingAccNavigationServices<AuthorizationPageVmd>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<AuthorizationPageVmd>());
        }

        private static INavigationService CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new SettingAccNavigationServices<RegistrationPageVmd>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<RegistrationPageVmd>());
        }

        private static INavigationService CreatePasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
        {
            return new SettingAccNavigationServices<PasswordRecoveryVmd>
                (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                    () => serviceProvider.GetRequiredService<PasswordRecoveryVmd>());
        }

        private static INavigationService CreateMainPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new NavigationServices<MainPageVmd>(serviceProvider.GetRequiredService<MainWindowNavigationStore>(),
                () => serviceProvider.GetRequiredService<MainPageVmd>());
        }

        private static INavigationService CreateSettingPageNavigationService(IServiceProvider serviceProvider)
        {
            return new AdditionalNavigationServices<SettingVmd>(serviceProvider.GetRequiredService<AdditionalNavigationStore>(), () => serviceProvider.GetRequiredService<SettingVmd>());
        }

        private static INavigationService CreateServerPageNavigationService(IServiceProvider serviceProvider)
        {
            return new MainPageServerNavigationServices<ServerVMD>(serviceProvider.GetRequiredService<MainPageServerNavigationStore>(), () => serviceProvider.GetRequiredService<ServerVMD>());
        }

        #endregion

        #region Модальные окна VMD

        private static INavigationService CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerRegistrationModalVmd>
            (serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<ServerRegistrationModalVmd>());
        }

        private static INavigationService CreateModalAuthorisationPageNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerAuthorizationModalVmd>(serviceProvider.GetRequiredService<ModalNavigationStore>(), () => serviceProvider.GetRequiredService<ServerAuthorizationModalVmd>());
        }

        private static INavigationService CreateModalPasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerPasswordRecoveryModalVmd>(serviceProvider.GetRequiredService<ModalNavigationStore>(), () => serviceProvider.GetRequiredService<ServerPasswordRecoveryModalVmd>());
        }

        #endregion

        #region Авторизация/Регистрация/восстановление пароля сервисы
        private static IRegistrationServices CreateApiRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationApiServerServices(serviceProvider.GetRequiredService<SavedServersStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(), 
                serviceProvider.GetRequiredService<CloseModalNavigationServices>(),
                serviceProvider.GetRequiredService<IHttpDataServices>());
        }

        private static IRegistrationServices CreateMainRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationMainServerService(serviceProvider.GetRequiredService<AccountStore>(),serviceProvider.GetRequiredService<IHttpDataServices>());
        }

        private static IAuthorizationServices CreateApiAuthorisationServices(IServiceProvider serviceProvider)
        {
            return new AuthorizationApiServerServices(serviceProvider.GetRequiredService<SavedServersStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(),
                serviceProvider.GetRequiredService<CloseModalNavigationServices>(),
                serviceProvider.GetRequiredService<IHttpDataServices>());
        }

       private static IAuthorizationServices CreateMainAuthorisationServices(IServiceProvider serviceProvider)
       {
            return new AuthoisationMainServerServices(serviceProvider.GetRequiredService<AccountStore>(),serviceProvider.GetRequiredService<IHttpDataServices>(),serviceProvider.GetRequiredService<IStatusServices>());
       }
      
        private static IAuthorizationServices? CreateAuthCheckApiServerSevices(IServiceProvider serviceProvider)
        {
            return new AuthCheckApiServerServices(serviceProvider.GetRequiredService<ServerAccountStore>(),serviceProvider.GetRequiredService<IHttpDataServices>());
        }

        private static ICaptchaServices CreateMainServeCaptchaServices(IServiceProvider serviceProvider)
        {
            return new MainServerCaptchaServices(serviceProvider.GetRequiredService<IHttpDataServices>(),
                serviceProvider.GetRequiredService<IImageServices>());
        }

        private static ICaptchaServices CreateApiServeCaptchaServices(IServiceProvider serviceProvider)
        {
            return new ApiServerCaptchaServices(serviceProvider.GetRequiredService<IHttpDataServices>(),
                serviceProvider.GetRequiredService<IImageServices>(),serviceProvider.GetRequiredService<CurrentServerStore>());
        }

        private static IGetPassRecoveryQuestionsServices CreateMainServerGetPasswordRecoveryQuestions(IServiceProvider serviceProvider)
        {
            return new MainServerGetPassRecQuestionsServices(serviceProvider.GetRequiredService<IHttpDataServices>());
        }

        private static IGetPassRecoveryQuestionsServices CreateApiServerGetPasswordRecoveryQuestions(IServiceProvider serviceProvider)
        {
            return new ApiServerGetPassRecQuestionsServices(serviceProvider.GetRequiredService<IHttpDataServices>(),serviceProvider.GetRequiredService<CurrentServerStore>());
        }


        private static IRecoveryPasswordServices CreateMainRecoveryPasswordServices(IServiceProvider serviceProvider)
        {
            return new MainServerRecoveryPasswordServices(serviceProvider.GetRequiredService<IHttpDataServices>());
        }

        private static IRecoveryPasswordServices CreateApiRecoveryPasswordServices(IServiceProvider serviceProvider)
        {
            return new ApiServerRecoveryPasswordServices(serviceProvider.GetRequiredService<IHttpDataServices>(),serviceProvider.GetRequiredService<CurrentServerStore>());
        }

        #endregion

      
    }
}
