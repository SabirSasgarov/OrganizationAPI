namespace OriganizationAPI.Extensions;

public static class FileExtension
{
	public static string SaveFile(this IFormFile file, string rootPath)
	{
		var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
		var filePath = Path.Combine(Directory.GetCurrentDirectory(), rootPath, fileName);
		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			file.CopyTo(stream);
		}
		return fileName;
	}
	public static void DeleteFile(string roothPath, string fileName)
	{
		var filePath = Path.Combine(Directory.GetCurrentDirectory(), roothPath, fileName);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}
	public static bool CheckFileType(this IFormFile file, string[] allowedTypes)
	{
		var fileExtension = Path.GetExtension(file.FileName).ToLower();
		return allowedTypes.Contains(fileExtension);
	}
}
