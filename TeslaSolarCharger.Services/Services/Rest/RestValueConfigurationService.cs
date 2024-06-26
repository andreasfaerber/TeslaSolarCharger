﻿using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TeslaSolarCharger.Model.Contracts;
using TeslaSolarCharger.Model.Entities.TeslaSolarCharger;
using TeslaSolarCharger.Services.Services.Rest.Contracts;
using TeslaSolarCharger.Shared.Dtos.RestValueConfiguration;
using TeslaSolarCharger.SharedBackend.MappingExtensions;

namespace TeslaSolarCharger.Services.Services.Rest;

public class RestValueConfigurationService(
    ILogger<RestValueConfigurationService> logger,
    ITeslaSolarChargerContext context,
    IMapperConfigurationFactory mapperConfigurationFactory) : IRestValueConfigurationService
{
    public async Task<List<DtoRestValueConfiguration>> GetAllRestValueConfigurations()
    {
        logger.LogTrace("{method}()", nameof(GetAllRestValueConfigurations));
        var mapper = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<RestValueConfiguration, DtoRestValueConfiguration>()
                ;
        });

        var result = await context.RestValueConfigurations
            .ProjectTo<DtoRestValueConfiguration>(mapper)
            .ToListAsync().ConfigureAwait(false);
        return result;
    }

    public async Task<List<DtoFullRestValueConfiguration>> GetFullRestValueConfigurationsByPredicate(
        Expression<Func<RestValueConfiguration, bool>> predicate)
    {
        logger.LogTrace("{method}({predicate})", nameof(GetFullRestValueConfigurationsByPredicate), predicate);
        var mapper = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<RestValueConfigurationHeader, DtoRestValueConfigurationHeader>();
            cfg.CreateMap<RestValueConfiguration, DtoFullRestValueConfiguration>()
                .ForMember(d => d.Headers, opt => opt.MapFrom(s => s.Headers))
                ;
        });
        var restValueConfigurations = await context.RestValueConfigurations
            .Where(predicate)
            .ProjectTo<DtoFullRestValueConfiguration>(mapper)
            .ToListAsync().ConfigureAwait(false);
        return restValueConfigurations;
    }

    public async Task<List<DtoJsonXmlResultConfiguration>> GetRestResultConfigurationByPredicate(
        Expression<Func<RestValueResultConfiguration, bool>> predicate)
    {

        var mapper = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<RestValueResultConfiguration, DtoJsonXmlResultConfiguration>()
                ;
        });
        var resultConfigurations = await context.RestValueResultConfigurations
            .Where(predicate)
            .ProjectTo<DtoJsonXmlResultConfiguration>(mapper)
            .ToListAsync().ConfigureAwait(false);
        return resultConfigurations;
    }

    public async Task<int> SaveRestValueConfiguration(DtoFullRestValueConfiguration dtoData)
    {
        logger.LogTrace("{method}({@dtoData})", nameof(SaveRestValueConfiguration), dtoData);
        var mapperConfiguration = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<DtoRestValueConfiguration, RestValueConfiguration>()
                ;
        });

        var mapper = mapperConfiguration.CreateMapper();
        var dbData = mapper.Map<RestValueConfiguration>(dtoData);
        if (dbData.Id == default)
        {
            context.RestValueConfigurations.Add(dbData);
        }
        else
        {
            var dtoHeaderIds = dtoData.Headers.Select(h => h.Id).ToList();
            var headersToRemove = await context.RestValueConfigurationHeaders
                .Where(x => x.RestValueConfigurationId == dbData.Id &&
                            !dtoHeaderIds.Contains(x.Id))
                .ToListAsync().ConfigureAwait(false);
            context.RestValueConfigurationHeaders.RemoveRange(headersToRemove);
            context.RestValueConfigurations.Update(dbData);
        }
        var headerMapperConfiguration = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<DtoRestValueConfigurationHeader, RestValueConfigurationHeader>()
                ;
        });
        var headerMapper = headerMapperConfiguration.CreateMapper();
        foreach (var dtoHeader in dtoData.Headers)
        {
            var dbHeader = headerMapper.Map<RestValueConfigurationHeader>(dtoHeader);
            if (dbData.Id == default)
            {
                dbData.Headers.Add(dbHeader);
            }
            else
            {
                dbHeader.RestValueConfigurationId = dbData.Id;
                if (dbHeader.Id == default)
                {
                    context.RestValueConfigurationHeaders.Add(dbHeader);
                }
                else
                {
                    context.RestValueConfigurationHeaders.Update(dbHeader);
                }
            }
        }
        await context.SaveChangesAsync().ConfigureAwait(false);
        return dbData.Id;
    }

    public async Task<List<DtoRestValueConfigurationHeader>> GetHeadersByConfigurationId(int parentId)
    {
        logger.LogTrace("{method}({parentId})", nameof(GetHeadersByConfigurationId), parentId);
        var mapper = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<RestValueConfigurationHeader, DtoRestValueConfigurationHeader>()
                ;
        });
        return await context.RestValueConfigurationHeaders
            .Where(x => x.RestValueConfigurationId == parentId)
            .ProjectTo<DtoRestValueConfigurationHeader>(mapper)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<int> SaveHeader(int parentId, DtoRestValueConfigurationHeader dtoData)
    {
        logger.LogTrace("{method}({@dtoData})", nameof(SaveHeader), dtoData);
        var mapperConfiguration = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<DtoRestValueConfigurationHeader, RestValueConfigurationHeader>()
                ;
        });

        var mapper = mapperConfiguration.CreateMapper();
        var dbData = mapper.Map<RestValueConfigurationHeader>(dtoData);
        dbData.RestValueConfigurationId = parentId;
        if (dbData.Id == default)
        {
            context.RestValueConfigurationHeaders.Add(dbData);
        }
        else
        {
            context.RestValueConfigurationHeaders.Update(dbData);
        }
        await context.SaveChangesAsync().ConfigureAwait(false);
        return dbData.Id;
    }

    public async Task DeleteHeader(int id)
    {
        logger.LogTrace("{method}({id})", nameof(DeleteHeader), id);
        context.RestValueConfigurationHeaders.Remove(new RestValueConfigurationHeader { Id = id });
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<List<DtoJsonXmlResultConfiguration>> GetResultConfigurationsByConfigurationId(int parentId)
    {
        logger.LogTrace("{method}({parentId})", nameof(GetResultConfigurationsByConfigurationId), parentId);
        var mapper = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<RestValueResultConfiguration, DtoJsonXmlResultConfiguration>()
                ;
        });
        return await context.RestValueResultConfigurations
            .Where(x => x.RestValueConfigurationId == parentId)
            .ProjectTo<DtoJsonXmlResultConfiguration>(mapper)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<int> SaveResultConfiguration(int parentId, DtoJsonXmlResultConfiguration dtoData)
    {
        logger.LogTrace("{method}({@dtoData})", nameof(SaveResultConfiguration), dtoData);
        var mapperConfiguration = mapperConfigurationFactory.Create(cfg =>
        {
            cfg.CreateMap<DtoJsonXmlResultConfiguration, RestValueResultConfiguration>()
                ;
        });

        var mapper = mapperConfiguration.CreateMapper();
        var dbData = mapper.Map<RestValueResultConfiguration>(dtoData);
        dbData.RestValueConfigurationId = parentId;
        if (dbData.Id == default)
        {
            context.RestValueResultConfigurations.Add(dbData);
        }
        else
        {
            context.RestValueResultConfigurations.Update(dbData);
        }
        await context.SaveChangesAsync().ConfigureAwait(false);
        return dbData.Id;
    }

    public async Task DeleteResultConfiguration(int id)
    {
        logger.LogTrace("{method}({id})", nameof(DeleteResultConfiguration), id);
        context.RestValueResultConfigurations.Remove(new RestValueResultConfiguration { Id = id });
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteRestValueConfiguration(int id)
    {
        logger.LogTrace("{method}({id})", nameof(DeleteRestValueConfiguration), id);
        var restValueConfiguration = await context.RestValueConfigurations
            .Include(x => x.Headers)
            .Include(x => x.RestValueResultConfigurations)
            .FirstAsync(x => x.Id == id).ConfigureAwait(false);
        context.RestValueConfigurations.Remove(restValueConfiguration);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
