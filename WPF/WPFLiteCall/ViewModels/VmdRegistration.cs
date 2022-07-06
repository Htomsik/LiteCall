﻿using System;
using Core.Services;
using Core.Services.AppInfrastructure.NavigationServices;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Services.Interfaces.Extra;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.AuthRegServices.Authorization;
using LiteCall.Services.AuthRegServices.Captcha;
using LiteCall.Services.AuthRegServices.PasswordRecovery;
using LiteCall.Services.AuthRegServices.Registration;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Pages;
using LiteCall.ViewModels.Pages.AutRegPasges;
using LiteCall.ViewModels.ServerPages;
using LiteCall.ViewModels.ServerPages.AuthRegPages;
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
                CreateAuthCheckApiServerServices(s),
                s.GetRequiredService<IStatusSc>(),
                s.GetRequiredService<IHttpDataServices>()));

        services.AddTransient(s => new SettingVmd(
            s.GetRequiredService<MainAccountStore>(),
            s.GetRequiredService<SavedServersStore>(), s.GetRequiredService<AppSettingsStore>(),
            CreateAutPageNavigationServices(s), s.GetRequiredService<IHttpDataServices>(),
            s.GetRequiredService<IStatusSc>(),
            s.GetRequiredService<SettingsAccNavigationStore>()
        ));


        services.AddTransient(s =>
            new ServerVmd(s.GetRequiredService<CurrentServerAccountStore>(), s.GetRequiredService<CurrentServerStore>(),
                s.GetRequiredService<IStatusSc>(), s.GetRequiredService<IChatServerSc>()));


        services.AddSingleton(s => new MainWindowVmd(
            s.GetRequiredService<MainWindowNavigationStore>(), s.GetRequiredService<AdditionalNavigationStore>(),
            s.GetRequiredService<ModalNavigationStore>(),
            s.GetRequiredService<AppExecutionStateStore>(),
            s.GetRequiredService<CloseModalNavigationSc>(),
            s.GetRequiredService<CloseAdditionalNavigationSc>(), s.GetRequiredService<IStatusSc>(),
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
        return new AdditionalNavigationSc<SettingVmd>(
            serviceProvider.GetRequiredService<AdditionalNavigationStore>(),
            serviceProvider.GetRequiredService<SettingVmd>);
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

    #endregion

    #region Авторизация/Регистрация/восстановление пароля сервисы

    private static IRegistrationSc CreateApiRegistrationServices(IServiceProvider serviceProvider)
    {
        return new RegistrationApiServerSc(serviceProvider.GetRequiredService<SavedServersStore>(),
            serviceProvider.GetRequiredService<CurrentServerStore>(),
            serviceProvider.GetRequiredService<CloseModalNavigationSc>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IRegistrationSc CreateMainRegistrationServices(IServiceProvider serviceProvider)
    {
        return new RegistrationMainServerSc(serviceProvider.GetRequiredService<MainAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IAuthorizationSc CreateApiAuthorizationServices(IServiceProvider serviceProvider)
    {
        return new AuthorizationApiServerSc(serviceProvider.GetRequiredService<SavedServersStore>(),
            serviceProvider.GetRequiredService<CurrentServerStore>(),
            serviceProvider.GetRequiredService<CloseModalNavigationSc>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IAuthorizationSc CreateMainAuthorizationServices(IServiceProvider serviceProvider)
    {
        return new AuthorizationMainServerSc(serviceProvider.GetRequiredService<MainAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<IStatusSc>());
    }

    private static IAuthorizationSc? CreateAuthCheckApiServerServices(IServiceProvider serviceProvider)
    {
        return new AuthCheckApiServerSc(serviceProvider.GetRequiredService<CurrentServerAccountStore>(),
            serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IGetCaptchaSc CreateMainServeCaptchaServices(IServiceProvider serviceProvider)
    {
        return new MainServerGetCaptchaSc(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<IImageServices>());
    }

    private static IGetCaptchaSc CreateApiServeCaptchaServices(IServiceProvider serviceProvider)
    {
        return new ApiServerGetCaptchaSc(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<IImageServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }

    private static IGetRecoveryQuestionsSc CreateMainServerGetPasswordRecoveryQuestions(
        IServiceProvider serviceProvider)
    {
        return new MainServerGetRecQuestionsSc(serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IGetRecoveryQuestionsSc CreateApiServerGetPasswordRecoveryQuestions(
        IServiceProvider serviceProvider)
    {
        return new ApiServerGetRecQuestionsSc(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }


    private static IRecoveryPasswordSc CreateMainRecoveryPasswordServices(IServiceProvider serviceProvider)
    {
        return new MainServerRecoveryPasswordSc(serviceProvider.GetRequiredService<IHttpDataServices>());
    }

    private static IRecoveryPasswordSc CreateApiRecoveryPasswordServices(IServiceProvider serviceProvider)
    {
        return new ApiServerRecoveryPasswordSc(serviceProvider.GetRequiredService<IHttpDataServices>(),
            serviceProvider.GetRequiredService<CurrentServerStore>());
    }

    #endregion
}