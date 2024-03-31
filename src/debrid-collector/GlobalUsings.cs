// Global using directives

global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Channels;
global using DebridCollector.Extensions;
global using DebridCollector.Features.Configuration;
global using DebridCollector.Features.Debrid;
global using DebridCollector.Features.Worker;
global using MassTransit;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Polly;
global using Polly.Extensions.Http;
global using SharedContracts.Configuration;
global using SharedContracts.Dapper;
global using SharedContracts.Extensions;
global using SharedContracts.Models;
global using SharedContracts.Python;
global using SharedContracts.Python.RTN;
global using SharedContracts.Requests;