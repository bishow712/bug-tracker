﻿using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Authentication;
using Server.Data;
using Server.Exceptions;
using System.Text;

namespace Server;

public static class Registry
{
	public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
	{
		builder.ConfigureIdentity();
		builder.ConfigureAuthentication();
		
		builder.Services.AddAntiforgery();

		builder.Services.AddProblemDetails();

		builder.Services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlite("Data Source=app.db");

		});

		builder.Services.AddCors(c => {
			c.AddPolicy("AllowOrigins", builder => {
				builder.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowAnyOrigin();
			});
		});

		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddAuthorization();

		builder.Services.AddExceptionHandler<CustomExceptionHandler>();

		builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

		builder.Services.AddCors();

		return builder;
	}

	public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
	{
		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKeyIsSecretSoDoNotTellAnyoneAboutIt")),
					ValidateIssuer = false,
					ValidateAudience = false,
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						context.Token = context.Request.Cookies["jwt"];
						return Task.CompletedTask;
					},
				};
			});

		return builder;
	}

	public static WebApplicationBuilder ConfigureIdentity(this WebApplicationBuilder builder)
	{
		builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
		{
			options.Password.RequireUppercase = false;
		})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		return builder;
	}
}
