using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace EmployManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{

                fonts.AddFont("FontAwesome6BrandsRegular400.otf", "FAB");
                fonts.AddFont("FontAwesome6FreeSolid900.otf", "FAS");
                fonts.AddFont("FontAwesome6FreeRegular400.otf", "FAR");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

