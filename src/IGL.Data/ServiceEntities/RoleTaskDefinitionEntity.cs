using IGL.Service.Common;
using System;

namespace IGL.Data.ServiceEntities
{
    public class RoleTaskDefinitionEntity : BaseTableEntity
    {
        public string Name { get; set; }
        public double Version { get; set; }
        public string QueueName { get; set; }
        public string Type { get; set; }

        public static implicit operator RoleTaskDefinition(RoleTaskDefinitionEntity from)
        {
            return AutoMapper.Mapper.Map<RoleTaskDefinitionEntity, RoleTaskDefinition>(from);
        }

        public static implicit operator RoleTaskDefinitionEntity(RoleTaskDefinition from)
        {
            return AutoMapper.Mapper.Map<RoleTaskDefinition, RoleTaskDefinitionEntity>(from);
        }
    }
}
