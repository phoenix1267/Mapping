using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class TextureLoader
{
    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f) {
   
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        if (FilePath.Length <= 0) return null;
        Texture2D SpriteTexture = LoadTexture(FilePath);
        if (SpriteTexture.IsUnityNull())
        {
            //Debug.Log(FilePath);   
            return null;
        }
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), PixelsPerUnit);
 
        return NewSprite;
    }
    
    public static Texture2D LoadTexture(string FilePath) 
    {
 
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails
 
        Texture2D Tex2D;
        byte[] FileData;
 
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if(Tex2D.LoadImage(FileData))         // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                   // If data = readable -> return texture
        }
        else
        {
            Debug.Log("Texture File Path is not valid");
        }
        return null;                     // Return null if load failed
    }
}
