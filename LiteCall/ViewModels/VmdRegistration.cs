using System;
using LiteCall.Services;
using LiteCall.Services.AuthRegServices.Authorization;
using LiteCall.Services.AuthRegServices.Captcha;
using LiteCall.Services.AuthRegServices.PasswordRecovery;
using LiteCall.Services.AuthRegServices.Registration;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.ViewModels.Pages;
using LiteCall.ViewModels.ServerPages;
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
            CreateMainAuthorizationServices(s), s.GetRequiredService<IEncryptServices>()));


        services.AddTransient(s => new RegistrationPageVmd(
            CreateAutPageNavigationServices(s),
            CreateMainRegistrationServices(s),
            s.GetRequiredService<IStatusServices>(),
            CreateMainServeCaptchaServices(s), CreateMainServerGetPasswordRecoveryQuestions(s),
            s.GetRequiredService<IEncryptServices>()));

        services.AddTransient(
            s => new PasswordRecoveryVmd(
                CreateAutPageNavigationServices(s),
                s.GetRequiredService<IStatusServices>(),
                CreateMainServerGetPasswordRecoveryQuestions(s),
                CreateMainRecoveryPasswordServices(s), s.GetRequiredService<IEncryptServices>()));

        #endregion

        #region Регистрация/Авторизация на API сервере чата

        services.AddTransient(s => new ServerRegistrationModalVmd(
            CreateModalAuthorizationPageNavigationService(s),
            CreateApiRegistrationServices(s),
            s.GetRequiredService<IStatusServices>(),
            CreateApiServeCaptchaServices(s), CreateApiServerGetPasswordRecoveryQuestions(s),
            s.GetRequiredService<IEncryptServices>()));


        services.AddTransient(s => new ServerAuthorizationModalVmd(
            CreateModalRegistrationPageNavigationServices(s),
            CreateModalPasswordRecoveryPageNavigationService(s),
            CreateApiAuthorizationServices(s), s.GetRequiredService<IEncryptServices>()));


        services.AddTransient(
            s => new ServerPasswordRecoveryModalVmd(
                CreateModalAuthorizationPageNavigationService(s),
                s.GetRequiredService<IStatusServices>(),
                CreateApiServerGetPasswordRecoveryQuestions(s),
                CreateApiRecoveryPasswordServices(s), s.GetRequiredService<IEncryptServices>()));

        #endregion

        #region Остальные VMD

        services.AddTransient(
            s => new MainPageVmd(s.GetRequiredService<AccountStore>(),
                s.GetRequiredService<ServerAccountStore>(),
                s.GetRequiredService<SavedServersStore>(),
                s.GetRequiredService<CurrentServerStore>(),
                s.GetRequiredService<MainPageServerNavigationStore>(),
                CreateSettingPageNavigationService(s),
                CreateServerPageNavigationService(s),
                CreateModalAuthorizationPageNavigationService(s),
                CreateAuthCheckApiServerServices(s),
                s.GetRequiredService<IStatusServices>(),
                s.GetRequiredService<IHttpDataServices>()));

        services.AddTransient(s => new SettingVmd(
            s.GetRequiredService<AccountStore>(),
            s.GetRequiredService<SavedServersStore>(), s.GetRequiredService<SettingsStore>(),
            CreateAutPageNavigationServices(s), s.GetRequiredService<IHttpDataServices>(),
            s.GetRequiredService<IStatusServices>(),
            s.GetRequiredService<SettingsAccNavigationStore>()
        ));


        services.AddTransient(s =>
            new ServerVmd(s.GetRequiredService<ServerAccountStore>(), s.GetRequiredService<CurrentServerStore>(),
                s.GetRequiredService<IStatusServices>()));


        services.AddSingleton(s => new MainWindowVmd(
            s.GetRequiredService<MainWindowNavigationStore>(), s.GetRequiredService<AdditionalNavigationStore>(),
            s.GetRequiredService<ModalNavigationStore>(),
            s.GetRequiredService<StatusMessageStore>(),
            s.GetRequiredService<CloseModalNavigationServices>(),
            s.GetRequiredService<CloseAdditionalNavigationServices>(), s.GetRequiredService<IStatusServices>(),
            s.GetRequiredService<ICloseAppServices>(), configuration));

        #endregion

        return services;
    }

    #region Создание навигационных сервисов для базовых VMD

    private static INavigationService CreateAutPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new SettingAccNavigationServices<AuthorizationPageVmd>
        (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
            serviceProvider.GetRequiredService<AuthorizationPageVmd>);
    }

    private static INavigationService CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new SettingAccNavigationServices<RegistrationPageVmd>
        (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
            serviceProvider.GetRequiredService<RegistrationPageVmd>);
    }

    private static INavigationService CreatePasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
    {
        return new SettingAccNavigationServices<PasswordRecoveryVmd>
        (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
            serviceProvider.GetRequiredService<PasswordRecoveryVmd>);
    }

    private static INavigationService CreateMainPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new NavigationServices<MainPageVmd>(serviceProvider.GetRequiredService<MainWindowNavigationStore>(),
            serviceProvider.GetRequiredService<MainPageVmd>);
    }

    private static INavigationService CreateSettingPageNavigationService(IServiceProvider serviceProvider)
    {
        return new AdditionalNavigationServices<SettingVmd>(
            serviceProvider.GetRequiredService<AdditionalNavigationStore>(),
            serviceProvider.GetRequiredService<SettingVmd>);
    }

    private static INavigationService CreateServerPageNavigationService(IServiceProvider serviceProvider)
    {
        return new MainPageServerNavigationServices<ServerVmd>(
            serviceProvider.GetRequiredService<MainPageServerNavigationStore>(),
            serviceProvider.GetRequiredService<ServerVmd>);
    }

    #endregion

    #region Модальные окна VMD

    private static INavigationService CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
    {
        return new ModalNavigateServices<ServerRegistrationModalVmd>
        (serviceProvider.GetRequiredService<ModalNavigationStore>(),
            serviceProvider.GetRequiredService<ServerRegistrationModalVmd>);
    }

    private static INavigationService CreateModalAuthorizationPageNavigationService(IServiceProvider serviceProvider)
    {
        return new ModalNavigateServices<ServerAuthorizationModalVmd>(
            serviceProvider.GetRequiredService<ModalNavigationStore>(),
            serviceProvider.GetRequiredService<ServerAuthorizationModalVmd>);
    }

    private static INavigationService CreateModalPasswordRecoveryPageNavigationService(IServiceProvider serviceProvider)
    {
        return new ModalNavigateServices<ServerPasswordRecoveryModalVmd>(
            serviceProvider.GetRequiredService<ModalNavigationStore>(),
            serviceProvider.GetRequiredService<ServerPasswordRecoveryModalVmd>);
    }

    #endregion

    #region Авторизация/Регистрация/восстановление пароля сервисы

    private static IRegistrationServices CreateApiRegistrationServices(IServiceProvider serviceProvider)
    {
        return new RegistrationApiServerServices(serviceProvider.GetRequiredService<SavedServersStore>(),
            serviceProvider.GetRequiredService<CurrentServerStore>(),
            serviceProvider.GetRequiredService<CloseModalNavigationServices>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IRegistrationServices CreateMainRegistrationServices(IServiceProvider serviceProvider)
    {
        return new RegistrationMainServerService(serviceProvider.GetRequiredService<AccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IAuthorizationServices CreateApiAuthorizationServices(IServiceProvider serviceProvider)
    {
        return new AuthorizationApiServerServices(serviceProvider.GetRequiredService<SavedServersStore>(),
            serviceProvider.GetRequiredService<CurrentServerStore>(),
            serviceProvider.GetRequiredService<CloseModalNavigationServices>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IAuthorizationServices CreateMainAuthorizationServices(IServiceProvider serviceProvider)
    {
        return new AuthorizationMainServerServices(serviceProvider.GetRequiredService<AccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<IStatusServices>());
    }

    private static IAuthorizationServices? CreateAuthCheckApiServerServices(IServiceProvider serviceProvider)
    {
        return new AuthCheckApiServerServices(serviceProvider.GetRequiredService<ServerAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static ICaptchaServices CreateMainServeCaptchaServices(IServiceProvider serviceProvider)
    {
        return new MainServerCaptchaServices(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<IImageServices>());
    }

    private static ICaptchaServices CreateApiServeCaptchaServices(IServiceProvider serviceProvider)
    {
        return new ApiServerCaptchaServices(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<IImageServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }

    private static IGetPassRecoveryQuestionsServices CreateMainServerGetPasswordRecoveryQuestions(
        IServiceProvider serviceProvider)
    {
        return new MainServerGetPassRecQuestionsServices(serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IGetPassRecoveryQuestionsServices CreateApiServerGetPasswordRecoveryQuestions(
        IServiceProvider serviceProvider)
    {
        return new ApiServerGetPassRecQuestionsServices(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }


    private static IRecoveryPasswordServices CreateMainRecoveryPasswordServices(IServiceProvider serviceProvider)
    {
        return new MainServerRecoveryPasswordServices(serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IRecoveryPasswordServices CreateApiRecoveryPasswordServices(IServiceProvider serviceProvider)
    {
        return new ApiServerRecoveryPasswordServices(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }

    #endregion
}