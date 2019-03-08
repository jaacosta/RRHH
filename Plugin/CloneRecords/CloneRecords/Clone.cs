using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloneRecords
{
    public class Clone : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("entityName") && context.InputParameters.Contains("entityId"))
            {
                string entityName = (string)context.InputParameters["entityName"];
                Guid entityId = new Guid((string)context.InputParameters["entityId"]);

                try
                {
                    Entity cloneFrom = service.Retrieve(entityName, entityId, new ColumnSet(true));
                    Entity cloneTo = new Entity(entityName);

                    foreach (KeyValuePair<string,object> attr in cloneFrom.Attributes)
                    {
                        if (attr.Key == "statecode" || attr.Key == "statuscode" || attr.Key == entityName + "id" || attr.Key == "ordernumber" || attr.Key == "new_cerrarrelevamiento" || attr.Key == "new_solicituddecosteoid" || attr.Key == "new_analistadesarrolloid" || attr.Key == "new_estadodelrelevamientocomercial")
                            continue;
                        cloneTo[attr.Key] = attr.Value;
                    }
                    cloneTo["new_relevamiento"] = "Duplicado-" + DateTime.Now.ToString() + "-" + cloneFrom.GetAttributeValue<string>("new_relevamiento");
                    Guid cloneToGuid = service.Create(cloneTo);
                    if (!Equals(cloneToGuid, Guid.Empty))
                    {
                        context.OutputParameters["cloneId"] = cloneToGuid.ToString();
                    }
                }

                catch (Exception ex)
                {
                    tracingService.Trace("CloneRecords: {0}", ex.ToString());
                    throw;
                }
            }

        }
    }
}
