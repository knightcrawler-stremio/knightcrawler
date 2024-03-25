// Global using directives

global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Channels;
global using MassTransit;
global using MassTransit.Mediator;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Polly;
global using Polly.Extensions.Http;
global using PromKnight.ParseTorrentTitle;
global using QBitCollector.Extensions;
global using QBitCollector.Features.Qbit;
global using QBitCollector.Features.Trackers;
global using QBitCollector.Features.Worker;
global using QBittorrent.Client;
global using SharedContracts.Configuration;
global using SharedContracts.Dapper;
global using SharedContracts.Extensions;
global using SharedContracts.Models;
global using SharedContracts.Requests;