using Application.Dtos.Users;
using Application.Dtos.Users.Responses;
using AutoMapper;
using Domain.Entities;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Application.MappingProfiles;

public class UserProfile : Profile
{
    private static IServiceProvider _serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public UserProfile() 
    {
        #region Responses
        CreateMap<User, DetailUserResponse>()
            .ConstructUsing((user, context) =>
            {
                var telegramBotClient = _serviceProvider.GetRequiredService<ITelegramBotClient>();
                var username = telegramBotClient.GetChatAsync(user.TelegramId).GetAwaiter().GetResult().Username;
                return new DetailUserResponse(
                    user.Id,
                    user.TelegramId,
                    username!,
                    user.Balance,
                    user.RangTap,
                    user.ReferalLink,
                    user.DateTimeRegistration,
                    HttpStatusCode.OK);
            });

        CreateMap<User, UserListItem>()
            .ConstructUsing((user, context) =>
            {
                var telegramBotClient = _serviceProvider.GetRequiredService<ITelegramBotClient>();
                var username = telegramBotClient.GetChatAsync(user.TelegramId).GetAwaiter().GetResult().Username;
                return new UserListItem(
                    user.Id,
                    user.TelegramId,
                    username!,
                    user.Balance,
                    user.RangTap);
            });

        
        CreateMap<IEnumerable<User>, ListUserResponse>()
            .ConstructUsing((users, context) =>
            {
                var userList = users.Select(u => context.Mapper.Map<UserListItem>(u)).ToList();
                return new ListUserResponse(HttpStatusCode.OK, userList);
            });
        #endregion

        #region Requests
        CreateMap<CreateUserRequest, User>()
            .ConstructUsing(dto => new User(dto.IdTelegram));
        #endregion
    }
}
