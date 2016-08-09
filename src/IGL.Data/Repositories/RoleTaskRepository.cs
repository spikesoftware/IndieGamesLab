﻿using IGL.Service.Common;

namespace IGL.Data.Repositories
{
    public class RoleTaskRepository : BaseTableRepository<ServiceEntities.RoleTaskDefinitionEntity>
    {
        // partition is the session of the game        
        // rowkey is a number generated by the client unique within the session of the game

        public RoleTaskRepository() : base("RoleTask", 100)
        {
            AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<RoleTaskDefinition, ServiceEntities.RoleTaskDefinitionEntity>()
                    .ForMember(m => m.PartitionKey, s => s.MapFrom(g => g.Name))
                    .ForMember(m => m.RowKey, s => s.MapFrom(g => g.Version));

                    cfg.CreateMap<ServiceEntities.RoleTaskDefinitionEntity, RoleTaskDefinition>();
                }
            );
        }

        public AzureResult InsertOrReplaceDefinition(RoleTaskDefinition definition)
        {
            return InsertOrReplace(definition);
        }
    }
}
