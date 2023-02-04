using Godot;
using System;

public class CanvasLayer : Godot.CanvasLayer
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("readyfunc");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	public void _on_Button_pressed()
	{
		print("onbuttonpressedfunc");
		WeatherForecastApi _apiClient = new WeatherForecastApi("https://rootingwebapi.azurewebsites.net/");
		var fc = await _apiClient.GetWeatherForecastAsync();
			return;
	}

}
