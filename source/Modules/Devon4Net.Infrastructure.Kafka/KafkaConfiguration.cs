﻿using System;
using System.Linq;
using Confluent.Kafka;
using Devon4Net.Infrastructure.Common.Enums;
using Devon4Net.Infrastructure.Common.Options;
using Devon4Net.Infrastructure.Common.Options.Kafka;
using Devon4Net.Infrastructure.Kafka.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devon4Net.Infrastructure.Kafka
{
    public static class KafkaConfiguration
    {
        private static ProducerConfig KafkaOptions { get; set; }

        public static void SetupKafka(this IServiceCollection services, IConfiguration configuration)
        {
            var kafkaOptions = services.GetTypedOptions<KafkaOptions>(configuration, OptionSectionName.KafkaSection);

            if (kafkaOptions == null || !kafkaOptions.EnableKafka || kafkaOptions.Producers == null || !kafkaOptions.Producers.Any()) return;

            services.AddTransient(typeof(IKakfkaHandler), typeof(KakfkaHandler));
        }

        public static void AddKafkaConsumer<T>(this IServiceCollection services, string consumerId, bool commit = false, int commitPeriod = 5) where T : class 
        {
            var memberInfo = typeof(T).BaseType;
            if (memberInfo != null && !memberInfo.Name.Contains("KafkaConsumerHandler"))
            {
                throw new ArgumentException($"The provided type {typeof(T).FullName} does not inherit from KafkaConsumerHandler");
            }

            using var sp = services.BuildServiceProvider();
            var kafHandler = sp.GetService<IKakfkaHandler>();

            var obj = (T)Activator.CreateInstance(typeof(T), services, kafHandler, consumerId, commit, commitPeriod);

            services.AddSingleton(obj);
        }

        public static void AddKafkaProducer<T>(this IServiceCollection services, string producerId) where T : class
        {
            var memberInfo = typeof(T).BaseType;
            if (memberInfo != null && !memberInfo.Name.Contains("KafkaProducerHandler"))
            {
                throw new ArgumentException($"The provided type {typeof(T).FullName} does not inherit from KafkaProducerHandler");
            }

            using var sp = services.BuildServiceProvider();
            var kafHandler = sp.GetService<IKakfkaHandler>();

            var obj = (T)Activator.CreateInstance(typeof(T), services, kafHandler, producerId);

            services.AddSingleton(obj);
        }
    }
}