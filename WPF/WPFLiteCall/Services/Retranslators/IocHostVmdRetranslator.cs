using System;
using Core.Services.Retranslators.Base;
using Core.VMD.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.Services.Retranslators;

public class IocHostVmdRetranslator : IRetranslor<Type,BaseVmd>
{
    public BaseVmd Retranslate(Type parameter) => (BaseVmd)App.Services.GetRequiredService(parameter);

}