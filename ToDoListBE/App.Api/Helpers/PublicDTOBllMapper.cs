using AutoMapper;

namespace WebApp.Helpers;

public class PublicDTOBllMapper<TLeftObject, TRightObject>  : IAppMapper<TLeftObject, TRightObject>
        where TRightObject : class where TLeftObject : class
    {
    private readonly IMapper _mapper;
    
    public PublicDTOBllMapper(IMapper mapper)
    {
        _mapper = mapper;
    }
    public TLeftObject? Map(TRightObject? inObject)
    {
        return _mapper.Map<TLeftObject>(inObject);
    }

    public TRightObject? Map(TLeftObject? inObject)
    {
        return _mapper.Map<TRightObject>(inObject);
    }
}
