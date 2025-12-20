using System.Net;
using System.Linq;
using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Application.Contracts.Infrastructure;
using Ecommerce.Application.Models.ImageManagement;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.ImageCloudinary;

public class ManageImageService : IManageImageService
{
    public CloudinarySettings _cloudinarySettings { get; }

    public ManageImageService(IOptions<CloudinarySettings> cloudinarySettings)
    {
        _cloudinarySettings = cloudinarySettings.Value;
    }

    private Cloudinary CreateClient()
    {
        var account = new Account(
            _cloudinarySettings.CloudName,
            _cloudinarySettings.ApiKey,
            _cloudinarySettings.ApiSecret
        );

        return new Cloudinary(account);
    }

    public async Task<ImageResponse> UploadImage(ImageData imageStream)
    {
        var cloudinary = CreateClient();

        var uploadImage = new ImageUploadParams()
        {
            File = new FileDescription(imageStream.Nombre, imageStream.ImageStream)
        };

        var uploadResult = await cloudinary.UploadAsync(uploadImage);

        if (uploadResult.StatusCode == HttpStatusCode.OK)
        {
            return new ImageResponse
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString()
            };
        }

        throw new Exception("No se pudo guardar la imagen");
    }

    public async Task DeleteImage(string? publicIdOrUrl)
    {
        if (string.IsNullOrWhiteSpace(publicIdOrUrl)) return;

        var publicId = ExtractPublicId(publicIdOrUrl);
        if (string.IsNullOrWhiteSpace(publicId)) return;

        var cloudinary = CreateClient();
        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image,
            Invalidate = true
        };
        await cloudinary.DestroyAsync(deletionParams);
    }

    private static string? ExtractPublicId(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        // If it's already a publicId, return without extension.
        if (!value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return RemoveExtension(value);
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)) return null;

        var path = uri.AbsolutePath;
        var uploadIndex = path.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);
        if (uploadIndex >= 0)
        {
            var afterUpload = path[(uploadIndex + "/upload/".Length)..];
            var segments = afterUpload.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (segments.Count > 0 && segments[0].StartsWith('v') && segments[0].Skip(1).All(char.IsDigit))
            {
                segments = segments.Skip(1).ToList();
            }
            var candidate = string.Join('/', segments);
            return RemoveExtension(candidate);
        }

        var segmentsFallback = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        var filename = string.Join('/', segmentsFallback);
        return RemoveExtension(filename);
    }

    private static string? RemoveExtension(string value)
    {
        var dotIndex = value.LastIndexOf('.');
        return dotIndex > 0 ? value[..dotIndex] : value;
    }
}
