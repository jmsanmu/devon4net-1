﻿using Devon4Net.Infrastructure.Common.Enums;
using Devon4Net.Infrastructure.Common.Options;
using Devon4Net.Infrastructure.Common.Options.SmaxHcm;
using Devon4Net.Infrastructure.SMAXHCM.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devon4Net.Application.WebAPI.Configuration
{
    public static class SmaxHcmConfiguration
    {
        public static void SetupSmaxHcm(this IServiceCollection services, ref IConfiguration configuration)
        {
            var smaxHcmOptions = services.GetTypedOptions<SmaxHcmOptions>(configuration, OptionSectionName.SmaxHcmSection);

            if (smaxHcmOptions == null || smaxHcmOptions.EnableSmax == false || string.IsNullOrEmpty(smaxHcmOptions.CircuitBreakerName) || string.IsNullOrEmpty(smaxHcmOptions.UserName) || string.IsNullOrEmpty(smaxHcmOptions.Password)) return;
            
            services.AddSingleton(typeof(ISmaxHcmHandler), typeof(SmaxHcmHandler));
        }
    }
}
