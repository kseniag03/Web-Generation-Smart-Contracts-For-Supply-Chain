using Application.DTOs;
using Core.UseCases;

namespace Application.Mappings
{
    public static class AbiBytecodeMapper
    {
        public static AbiBytecodeDto ToDto(this DeploySmartContractEntities abiBytecode)
        {
            return new AbiBytecodeDto
            {
                Abi = abiBytecode.Abi,
                Bytecode = abiBytecode.Bytecode
            };
        }
    }
}
