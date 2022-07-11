using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Infrastructure;
using SingleViewApi.V1.UseCase;
using SingleViewApi.V1.UseCase.Interfaces;
using SingleViewApi.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using Hackney.Core.Logging;
using Hackney.Core.Middleware.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Hackney.Core.HealthCheck;
using Hackney.Core.Middleware.CorrelationId;
using Hackney.Core.DynamoDb.HealthCheck;
using Hackney.Core.JWT;
using Hackney.Core.Middleware.Exception;
using ServiceStack.Redis;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.Helpers;
using SingleViewApi.V1.Helpers.Interfaces;


namespace SingleViewApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            AWSSDKHandler.RegisterXRayForAllServices();
        }

        public IConfiguration Configuration { get; }
        private static List<ApiVersionDescription> ApiVersions { get; set; }
        private const string ApiName = "Single View API";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc().AddNewtonsoftJson();
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddHttpClient();

            services.AddHttpContextAccessor();

            services.AddTransient<IPersonGateway, PersonGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new PersonGateway(
                    httpClient,
                    Environment.GetEnvironmentVariable("PERSON_API_V1")
                    );
            });

            services.AddTransient<IContactDetailsGateway, ContactDetailsGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new ContactDetailsGateway(
                    httpClient,
                    Environment.GetEnvironmentVariable("CONTACT_DETAILS_API_V2")
                    );
            });

            services.AddTransient<ICautionaryAlertsGateway, CautionaryAlertsGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new CautionaryAlertsGateway(
                    httpClient,
                    Environment.GetEnvironmentVariable("CAUTIONARY_ALERTS_API")
                    );
            });

            services.AddTransient<IEqualityInformationGateway, EqualityInformationGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new EqualityInformationGateway(httpClient,
                    Environment.GetEnvironmentVariable("EQUALITY_INFORMATION_API_V1"));
            });

            services.AddTransient<IGetPersonApiByIdUseCase, GetPersonApiByIdUseCase>(s =>
            {
                var personGateway = s.GetService<IPersonGateway>();
                var contactDetailsGateway = s.GetService<IContactDetailsGateway>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                var equalityInformationGateway = s.GetService<IEqualityInformationGateway>();
                var cautionaryAlertsGateway = s.GetService<ICautionaryAlertsGateway>();

                return new GetPersonApiByIdUseCase(personGateway, contactDetailsGateway, dataSourceGateway, equalityInformationGateway, cautionaryAlertsGateway);
            });

            services.AddTransient<ICreateCustomerUseCase, CreateCustomerUseCase>(s =>
            {
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                var customerGateway = s.GetService<ICustomerGateway>();
                var customerDataSourceGateway = s.GetService<ICustomerDataSourceGateway>();

                return new CreateCustomerUseCase(customerGateway, customerDataSourceGateway, dataSourceGateway);
            });

            services.AddTransient<IHousingSearchGateway, HousingSearchGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new HousingSearchGateway(httpClient,
                    Environment.GetEnvironmentVariable("HOUSING_SEARCH_API_V1"));
            });

            services.AddTransient<IRedisGateway, RedisGateway>(s =>
            {
                var host = Environment.GetEnvironmentVariable("REDIS_HOST");

                var redisManager = new RedisManagerPool(host);

                return new RedisGateway(
                    redisManager.GetClient());
            });

            services.AddHttpClient("JigsawClient").ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("JIGSAW_LOGIN_URL"));

            }).ConfigurePrimaryHttpMessageHandler(
                () => new HttpClientHandler() { CookieContainer = new CookieContainer() });

            services.AddTransient<IJigsawGateway, JigsawGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient("JigsawClient");

                return new JigsawGateway(httpClient, Environment.GetEnvironmentVariable("JIGSAW_LOGIN_URL"), Environment.GetEnvironmentVariable("JIGSAW_CUSTOMER_API"));

            });

            services.AddTransient<IGetCustomerByIdUseCase, GetCustomerByIdUseCase>(s =>
            {
                var customerGateway = s.GetService<ICustomerGateway>();
                var getPersonApiByIdUseCase = s.GetService<IGetPersonApiByIdUseCase>();
                var getJigsawCustomerByIdUseCase = s.GetService<IGetJigsawCustomerByIdUseCase>();
                var getCouncilTaxAccountByIdAccountRefUseCase = s.GetService<IGetCouncilTaxAccountByAccountRefUseCase>();

                return new GetCustomerByIdUseCase(customerGateway, getPersonApiByIdUseCase, getJigsawCustomerByIdUseCase, getCouncilTaxAccountByIdAccountRefUseCase);
            });
            services.AddTransient<IStoreJigsawCredentialsUseCase, StoreJigsawCredentialsUseCase>(s =>
            {
                var redisGateway = s.GetService<IRedisGateway>();
                var jigsawGateway = s.GetService<IJigsawGateway>();
                var decoderHelper = s.GetService<IDecoderHelper>();
                return new StoreJigsawCredentialsUseCase(redisGateway, jigsawGateway, decoderHelper);
            });

            services.AddTransient<IGetJigsawAuthTokenUseCase, GetJigsawAuthTokenUseCase>(s =>
            {
                var jigsawGateway = s.GetService<IJigsawGateway>();
                var redisGateway = s.GetService<IRedisGateway>();
                var decoderHelper = s.GetService<IDecoderHelper>();
                return new GetJigsawAuthTokenUseCase(jigsawGateway, redisGateway, decoderHelper);
            });

            services.AddTransient<IGetJigsawCustomersUseCase, GetJigsawCustomersUseCase>(s =>
            {
                var jigsawGateway = s.GetService<IJigsawGateway>();
                var jigsawAuthUseCase = s.GetService<IGetJigsawAuthTokenUseCase>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();

                return new GetJigsawCustomersUseCase(jigsawGateway, jigsawAuthUseCase, dataSourceGateway);
            });

            services.AddTransient<IGetJigsawCustomerByIdUseCase, GetJigsawCustomerByIdUseCase>(s =>
            {
                var jigsawGateway = s.GetService<IJigsawGateway>();
                var jigsawAuthUseCase = s.GetService<IGetJigsawAuthTokenUseCase>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();

                return new GetJigsawCustomerByIdUseCase(jigsawGateway, jigsawAuthUseCase, dataSourceGateway);
            });

            services.AddTransient<IGetSearchResultsByNameUseCase, GetSearchResultsByNameUseCase>(s =>
            {
                var housingSearchGateway = s.GetService<IHousingSearchGateway>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                return new GetSearchResultsByNameUseCase(housingSearchGateway, dataSourceGateway);
            });

            services.AddTransient<ISearchSingleViewUseCase, SearchSingleViewUseCase>(s =>
            {
                var customerGateway = s.GetService<ICustomerGateway>();

                return new SearchSingleViewUseCase(customerGateway);
            });

            services.AddTransient<IGetCombinedSearchResultsByNameUseCase, GetCombinedSearchResultsByNameUseCase>(s =>
            {
                var getSearchResultsByNameUseCase = s.GetService<IGetSearchResultsByNameUseCase>();
                var getJigsawCustomersUseCase = s.GetService<IGetJigsawCustomersUseCase>();
                var searchSingleViewUseCase = s.GetService<ISearchSingleViewUseCase>();
                var getCouncilTaxAccountsByCustomerNameUseCase = s.GetService<IGetCouncilTaxAccountsByCustomerNameUseCase>();
                var getHousingBenefitsAccountsByCustomerNameUseCase = s.GetService<IGetHousingBenefitsAccountsByCustomerNameUseCase>();
                return new GetCombinedSearchResultsByNameUseCase(getSearchResultsByNameUseCase, getJigsawCustomersUseCase, searchSingleViewUseCase, getCouncilTaxAccountsByCustomerNameUseCase, getHousingBenefitsAccountsByCustomerNameUseCase);
            });

            services.AddTransient<IGetCouncilTaxAccountByAccountRefUseCase, IGetCouncilTaxAccountByAccountRefUseCase>(s =>
            {
                var academyGateway = s.GetService<IAcademyGateway>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                return new GetCouncilTaxAccountByIdUseCase(academyGateway, dataSourceGateway);
            });

            services.AddTransient<INotesGateway, NotesGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new NotesGateway(
                    httpClient,
                    Environment.GetEnvironmentVariable("NOTES_API_V2")
                );
            });

            services.AddTransient<IDecoderHelper>(s =>
            {
                return new DecoderHelper(Environment.GetEnvironmentVariable(("RSA_PRIVATE_KEY")));
            });

            services.AddTransient<IGetNotesUseCase>(s =>
            {
                var notesGateway = s.GetService<INotesGateway>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                return new GetNotesUseCase(notesGateway, dataSourceGateway);
            });

            services.AddTransient<IGetJigsawNotesUseCase>(s =>
            {
                var jigsawGateway = s.GetService<IJigsawGateway>();
                var getJigsawAuthTokenUseCase = s.GetService<IGetJigsawAuthTokenUseCase>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                return new GetJigsawNotesUseCase(jigsawGateway, getJigsawAuthTokenUseCase, dataSourceGateway);
            });

            services.AddTransient<IGetAllNotesUseCase, GetAllNotesUseCase>(s =>
            {
                var getNotesUseCase = s.GetService<IGetNotesUseCase>();
                var getJigsawNotesUseCase = s.GetService<IGetJigsawNotesUseCase>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();

                return new GetAllNotesUseCase(getNotesUseCase, getJigsawNotesUseCase, dataSourceGateway);
            });

            services.AddTransient<ICreateNoteUseCase, CreateNoteUseCase>(s =>
            {
                var notesGateway = s.GetService<INotesGateway>();
                return new CreateNoteUseCase(notesGateway);
            });

            services.AddTransient<IAcademyGateway, AcademyGateway>(s =>
            {
                var httpClient = s.GetService<IHttpClientFactory>().CreateClient();

                return new AcademyGateway(
                    httpClient,
                    Environment.GetEnvironmentVariable("ACADEMY_API_V1")
                );
            });

            services.AddTransient<IGetCouncilTaxAccountsByCustomerNameUseCase, GetCouncilTaxAccountsByCustomerNameUseCase>(s =>
            {
                var academyGateway = s.GetService<IAcademyGateway>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();

                return new GetCouncilTaxAccountsByCustomerNameUseCase(academyGateway, dataSourceGateway);

            });

            services.AddTransient<IGetHousingBenefitsAccountsByCustomerNameUseCase, GetHousingBenefitsAccountsByCustomerNameUseCase>(s =>
            {
                var academyGateway = s.GetService<IAcademyGateway>();
                var dataSourceGateway = s.GetService<IDataSourceGateway>();
                return new GetHousingBenefitsAccountsByCustomerNameUseCase(academyGateway, dataSourceGateway);
            });

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddHealthChecks();

            services.AddTokenFactory();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Token",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney API Key",
                        Name = "X-Api-Key",
                        Type = SecuritySchemeType.ApiKey
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Token" }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var apiVersion in ApiVersions)
                {
                    var version = $"v{apiVersion.ApiVersion.ToString()}";
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{ApiName}-api {version}",
                        Version = version,
                        Description = $"{ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                c.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            services.ConfigureLambdaLogging(Configuration);

            services.AddLogCallAspect();

            ConfigureDbContext(services);
            //TODO: For DynamoDb, remove the line above and uncomment the line below.
            //services.ConfigureDynamoDB();

            RegisterGateways(services);
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            services.AddDbContext<SingleViewContext>(
                opt => opt.UseNpgsql(connectionString)
                // TODO: re-enable this in the future
                //.AddXRayInterceptor(true)
                );
        }



        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddScoped<IDataSourceGateway, DataSourceGateway>();
            services.AddScoped<ICustomerDataSourceGateway, CustomerDataSourceGateway>();
            services.AddScoped<ICustomerGateway, CustomerGateway>();

            //TODO: For DynamoDb, remove the line above and uncomment the line below.
            //services.AddScoped<IExampleDynamoGateway, DynamoDbGateway>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseCors(builder => builder
                  .AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("x-correlation-id"));

            app.UseCorrelationId();
            app.UseLoggingScope();
            app.UseCustomExceptionHandler(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // AWS XRay tracing
            app.UseXRay("SingleViewApi");


            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            ApiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in ApiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHealthChecks("/api/v1/healthcheck/ping", new HealthCheckOptions()
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteResponse
                });
            });
            app.UseLogCall();
        }
    }
}
