using System;
using Core.Services;
using Core.Services.AccountManagement.Authorization;
using Core.Services.AccountManagement.Captcha;
using Core.Services.AccountManagement.PasswordRecovery;
using Core.Services.AccountManagement.PasswordRecovery.Questions;
using Core.Services.AccountManagement.Registration;
using Core.Services.AppInfrastructure.NavigationServices;
using Core.Services.Connections;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Services.Interfaces.Extra;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Pages.AccountManagement;
using Core.VMD.Pages.AccountManagement.ChatServer;
using Core.VMD.Pages.Single;
using Core.VMD.ServerPages;
using Core.VMD.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.ViewModels;

internal static class VmdRegistration
{
    public static IServiceCollection RegisterVmd(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(CreateMainPageNavigationServices);

        #region Регистрация/Авторизация/Восстановление пароля на мейн сервере

        services.AddTransient(s => new AuthorizationPageVmd(
            CreateRegistrationPageNavigationServices(s), CreatePasswordRecoveryPageNavigationService(s),
            CreateMainAuthorizationServices(s), s.GetRequiredService<IEncryptSc>()));


        services.AddTransient(s => new RegistrationPageVmd(
            CreateAutPageNavigationServices(s),
            CreateMainRegistrationServices(s),
            s.GetRequiredService<IStatusSc>(),
            CreateMainServeCaptchaServices(s), CreateMainServerGetPasswordRecoveryQuestions(s),
            s.GetRequiredService<IEncryptSc>()));

        services.AddTransient(
            s => new PasswordRecoveryVmd(
                CreateAutPageNavigationServices(s),
                s.GetRequiredService<IStatusSc>(),
                CreateMainServerGetPasswordRecoveryQuestions(s),
                CreateMainRecoveryPasswordServices(s), s.GetRequiredService<IEncryptSc>()));

        #endregion

        #region Регистрация/Авторизация на API сервере чата

        services.AddTransient(s => new ServerRegistrationModalVmd(
            CreateModalAuthorizationPageNavigationService(s),
            CreateApiRegistrationServices(s),
            s.GetRequiredService<IStatusSc>(),
            CreateApiServeCaptchaServices(s), CreateApiServerGetPasswordRecoveryQuestions(s),
            s.GetRequiredService<IEncryptSc>()));


        services.AddTransient(s => new ServerAuthorizationModalVmd(
            CreateModalRegistrationPageNavigationServices(s),
            CreateModalPasswordRecoveryPageNavigationService(s),
            CreateApiAuthorizationServices(s), s.GetRequiredService<IEncryptSc>()));


        services.AddTransient(
            s => new ServerPasswordRecoveryModalVmd(
                CreateModalAuthorizationPageNavigationService(s),
                s.GetRequiredService<IStatusSc>(),
                CreateApiServerGetPasswordRecoveryQuestions(s),
                CreateApiRecoveryPasswordServices(s), s.GetRequiredService<IEncryptSc>()));

        #endregion

        #region Остальные VMD

        services.AddTransient(
            s => new MainPageVmd(s.GetRequiredService<MainAccountStore>(),
                s.GetRequiredService<CurrentServerAccountStore>(),
                s.GetRequiredService<SavedServersStore>(),
                s.GetRequiredService<CurrentServerStore>(),
                s.GetRequiredService<MainPageServerNavigationStore>(),
                CreateSettingPageNavigationService(s),
                CreateServerPageNavigationService(s),
                CreateModalAuthorizationPageNavigationService(s),
                CreateModalServerConnectionNavigationService(s),
                CreateAuthCheckApiServerServices(s),
                s.GetRequiredService<IStatusSc>(),
                s.GetRequiredService<IHttpDataSc>()));

        services.AddTransient(s => new SettingsPageVmd(
            s.GetRequiredService<MainAccountStore>(),
            s.GetRequiredService<SavedServersStore>(), s.GetRequiredService<AppSettingsStore>(),
            CreateAutPageNavigationServices(s), s.GetRequiredService<IHttpDataSc>(),
            s.GetRequiredService<IStatusSc>(),
            s.GetRequiredService<SettingsAccNavigationStore>()
        ));
        
        
        services.AddTransient(s =>
            new ServerVmd(s.GetRequiredService<CurrentServerAccountStore>(), 
                s.GetRequiredService<CurrentServerStore>(),
                s.GetRequiredService<IStatusSc>(), s.GetRequiredService<IChatServerSc>()));


        services.AddTransient(s=> new ServerConnectionVmd(CreateAuthCheckApiServerServices(s),
            s.GetRequiredService<SavedServersStore>(),
            s.GetRequiredService<MainAccountStore>(),
            s.GetRequiredService<CurrentServerStore>(),s.GetRequiredService<IHttpDataSc>(),CreateServerPageNavigationService(s),s.GetRequiredService<CloseModalNavigationSc>()));


        services.AddSingleton(s => new MainWindowVmd(
            s.GetRequiredService<MainWindowNavigationStore>(), s.GetRequiredService<AdditionalNavigationStore>(),
            s.GetRequiredService<ModalNavigationStore>(),
            s.GetRequiredService<AppExecutionStateStore>(),
            s.GetRequiredService<CloseModalNavigationSc>(),
            s.GetRequiredService<CloseAdditionalNavigationSc>(), 
            s.GetRequiredService<ICloseAppSc>(), configuration));

        #endregion

        return services;
    }

    #region Создание навигационных сервисов для базовых VMD

    private static INavigationSc CreateAutPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new SettingAccNavigationSc<AuthorizationPageVmd>
        (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
            serviceProvider.GetRequiredService<AuthorizationPageVmd>);
    }
    private static INavigationSc CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new SettingAccNavigationSc<RegistrationPageVmd>
        (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
            serviceProvider.GetRequiredService<RegistrationPageVmd>);
    }

    private static INavigationSc CreatePasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
    {
        return new SettingAccNavigationSc<PasswordRecoveryVmd>
        (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
            serviceProvider.GetRequiredService<PasswordRecoveryVmd>);
    }

    private static INavigationSc CreateMainPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new NavigationSc<MainPageVmd>(serviceProvider.GetRequiredService<MainWindowNavigationStore>(),
            serviceProvider.GetRequiredService<MainPageVmd>);
    }

    private static INavigationSc CreateSettingPageNavigationService(IServiceProvider serviceProvider)
    {
        return new AdditionalNavigationSc<SettingsPageVmd>(
            serviceProvider.GetRequiredService<AdditionalNavigationStore>(),
            serviceProvider.GetRequiredService<SettingsPageVmd>);
    }

    private static INavigationSc CreateServerPageNavigationService(IServiceProvider serviceProvider)
    {
        return new MainPageServerNavigationScs<ServerVmd>(
            serviceProvider.GetRequiredService<MainPageServerNavigationStore>(),
            serviceProvider.GetRequiredService<ServerVmd>);
    }

    #endregion

    #region Модальные окна VMD

    private static INavigationSc CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new ModalNavigateSc<ServerRegistrationModalVmd>
        (serviceProvider.GetRequiredService<ModalNavigationStore>(),
            serviceProvider.GetRequiredService<ServerRegistrationModalVmd>);
    }

    private static INavigationSc CreateModalAuthorizationPageNavigationService(IServiceProvider serviceProvider)
    {
        return new ModalNavigateSc<ServerAuthorizationModalVmd>(
            serviceProvider.GetRequiredService<ModalNavigationStore>(),
            serviceProvider.GetRequiredService<ServerAuthorizationModalVmd>);
    }

    private static INavigationSc CreateModalPasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
    {
        return new ModalNavigateSc<ServerPasswordRecoveryModalVmd>(
            serviceProvider.GetRequiredService<ModalNavigationStore>(),
            serviceProvider.GetRequiredService<ServerPasswordRecoveryModalVmd>);
    }
    
    private static INavigationSc CreateModalServerConnectionNavigationService(IServiceProvider serviceProvider)
    {
        return new ModalNavigateSc<ServerConnectionVmd>(serviceProvider.GetRequiredService<ModalNavigationStore>(), serviceProvider.GetRequiredService<ServerConnectionVmd>);
    }

    #endregion

    #region Авторизация/Регистрация/восстановление пароля сервисы

    private static IRegistrationSc CreateApiRegistrationServices(IServiceProvider serviceProvider)
    {
        return new RegistrationApiServerSc(serviceProvider.GetRequiredService<SavedServersStore>(),
            serviceProvider.GetRequiredService<CurrentServerStore>(),
            serviceProvider.GetRequiredService<CloseModalNavigationSc>(),
            serviceProvider.GetRequiredService<IHttpDataSc>());
    }

    private static IRegistrationSc CreateMainRegistrationServices(IServiceProvider serviceProvider)
    {
        return new RegistrationMainServerSc(serviceProvider.GetRequiredService<MainAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataSc>());
    }

    private static IAuthorizationSc CreateApiAuthorizationServices(IServiceProvider serviceProvider)
    {
        return new AuthorizationApiServerSc(serviceProvider.GetRequiredService<SavedServersStore>(),
            serviceProvider.GetRequiredService<CurrentServerStore>(),
            serviceProvider.GetRequiredService<CloseModalNavigationSc>(),
            serviceProvider.GetRequiredService<IHttpDataSc>());
    }

    private static IAuthorizationSc CreateMainAuthorizationServices(IServiceProvider serviceProvider)
    {
        return new AuthorizationMainServerSc(serviceProvider.GetRequiredService<MainAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataSc>(),
            serviceProvider.GetRequiredService<IStatusSc>());
    }

    private static IAuthorizationSc? CreateAuthCheckApiServerServices(IServiceProvider serviceProvider)
    {
        return new AuthCheckApiServerSc(serviceProvider.GetRequiredService<CurrentServerAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataSc>());
    }

    private static IGetCaptchaSc CreateMainServeCaptchaServices(IServiceProvider serviceProvider)
    {
        return new MainServerGetCaptchaSc(serviceProvider.GetRequiredService<IHttpDataSc>(),
            serviceProvider.GetRequiredService<IImageServices>());
    }

    private static IGetCaptchaSc CreateApiServeCaptchaServices(IServiceProvider serviceProvider)
    {
        return new ApiServerGetCaptchaSc(serviceProvider.GetRequiredService<IHttpDataSc>(),
            serviceProvider.GetRequiredService<IImageServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }

    private static IGetRecoveryQuestionsSc CreateMainServerGetPasswordRecoveryQuestions(
        IServiceProvider serviceProvider)
    {
        return new MainServerGetRecQuestionsSc(serviceProvider.GetRequiredService<IHttpDataSc>());
    }

    private static IGetRecoveryQuestionsSc CreateApiServerGetPasswordRecoveryQuestions(
        IServiceProvider serviceProvider)
    {
        return new ApiServerGetRecQuestionsSc(serviceProvider.GetRequiredService<IHttpDataSc>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }


    private static IRecoveryPasswordSc CreateMainRecoveryPasswordServices(IServiceProvider serviceProvider)
    {
        return new MainServerRecoveryPasswordSc(serviceProvider.GetRequiredService<IHttpDataSc>());
    }

    private static IRecoveryPasswordSc CreateApiRecoveryPasswordServices(IServiceProvider serviceProvider)
    {
        return new ApiServerRecoveryPasswordSc(serviceProvider.GetRequiredService<IHttpDataSc>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }

    #endregion
}