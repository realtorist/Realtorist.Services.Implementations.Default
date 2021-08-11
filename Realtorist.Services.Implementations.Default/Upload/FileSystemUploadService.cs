using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Realtorist.Models.Helpers;
using Realtorist.Models.Media;
using Realtorist.Models.Pagination;
using Realtorist.Services.Abstractions.Providers;
using Realtorist.Services.Abstractions.Upload;

namespace Realtorist.Services.Implementations.Default.Upload
{
    /// <summary>
    /// Provides operations for file uploads using file system
    /// </summary>
    public class FileSystemUploadService : IUploadService
    {
        private const string UploadsFolder = "uploads";
        private const string PathToFolder = "wwwroot/" + UploadsFolder;

        private readonly ILinkProvider _linkProvider;
        private readonly ILogger _logger;

        public FileSystemUploadService(ILinkProvider linkProvider, ILogger<FileSystemUploadService> logger)
        {
            _linkProvider = linkProvider ?? throw new ArgumentNullException(nameof(linkProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginationResult<MediaFile>> GetFilesAsync(PaginationRequest request)
        {
            var result = new PaginationResult<MediaFile>
            {
                Limit = request.Limit,
                Offset = request.Offset,
            };

            var directory = new DirectoryInfo(PathToFolder);
            var files = directory.EnumerateFileSystemInfos("*.*", SearchOption.AllDirectories)
                .Where(file => file.Name.ToLower().EndsWith(".jpg") 
                    || file.Name.ToLower().EndsWith(".jpeg") 
                    || file.Name.ToLower().EndsWith(".png") 
                    || file.Name.ToLower().EndsWith(".gif") 
                    || file.Name.ToLower().EndsWith(".svg"))
                .ToList();

            if (request.SortField.IsNullOrEmpty())
            {
                request.SortField = nameof(MediaFile.CreatedAt);
                request.SortOrder = Models.Enums.SortByOrder.Desc;
            }

            result.TotalRecords = files.Count;

            var query = files.Select(file => new MediaFile { 
                    Url = _linkProvider.GetAbsoluteLink($"~/{UploadsFolder}/{file.Name}"),
                    CreatedAt = file.CreationTime,
                    Size = new FileInfo(file.FullName).Length,
                    Name = file.Name,
                    Id = file.Name
                });

            var sortFunc = request.SortField.GetSelectExpression<MediaFile>().Compile();
            if (request.SortOrder == Models.Enums.SortByOrder.Desc)
            {
                query = query.OrderByDescending(sortFunc);
            }
            else
            {
                query = query.OrderBy(sortFunc);
            }

            result.Results = query
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToArray();

            _logger.LogInformation($"Found {result.TotalRecords} files in storage. Returning {result.RecordsReturned} files");

            return result;
        }

        public async Task<IEnumerable<MediaFile>> GetFilesAsync()
        {
            var directory = new DirectoryInfo(PathToFolder);
            var files = directory.EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Where(file => file.Name.ToLower().EndsWith(".jpg") 
                    || file.Name.ToLower().EndsWith(".jpeg") 
                    || file.Name.ToLower().EndsWith(".png") 
                    || file.Name.ToLower().EndsWith(".gif") 
                    || file.Name.ToLower().EndsWith(".svg"))
                .OrderByDescending(file => file.CreationTime)    
                .Select(file => new MediaFile { 
                    Url = _linkProvider.GetAbsoluteLink($"~/{UploadsFolder}/{file.Name}"),
                    CreatedAt = file.CreationTime,
                    Size = file.FullName.Length,
                    Name = file.Name,
                    Id = file.Name
                })
                .ToList();

            _logger.LogInformation($"Found {files.Count} files in storage.");

            return files;
        }

        public async Task<MediaFile> UploadFileAsync(Stream file, string originalName)
        {
            var name = Guid.NewGuid().ToString() + "." + originalName.Substring(originalName.LastIndexOf(".")+1);
            using (var writeStream = System.IO.File.Create(PathToFolder + "/" + name)) 
            {
                await file.CopyToAsync(writeStream);

                _logger.LogInformation($"Uploaded new file '{name}' with size {writeStream.Length} bytes to file storage");
                return new MediaFile { 
                    Url = _linkProvider.GetAbsoluteLink($"~/{UploadsFolder}/{name}"),
                    CreatedAt = DateTime.Now,
                    Size = writeStream.Length,
                    Name = name,
                    Id = name
                };
            }
        }
        
        public async Task DeleteFileAsync(string id)
        {
            File.Delete($"{PathToFolder}/{id}");
            _logger.LogInformation($"Removed file with id '{id}");
        }
    }
}