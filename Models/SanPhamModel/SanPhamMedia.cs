using System;

namespace MVCPlayWithMe.Models.SanPhamModel
{
    public class SanPhamMedia
    {
        public int Id { get; set; }

        public int SanPhamId { get; set; }

        /// <summary>
        /// Loại media: "image" hoặc "video"
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Tên file: anh.webp, video.mp4
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Title hiển thị - dùng làm Caption cho image, Title cho video
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Alt text cho SEO - chỉ dùng cho image, video = null
        /// </summary>
        public string AltText { get; set; }

        /// <summary>
        /// Mô tả chi tiết - chỉ dùng cho video (schema markup), image = null
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ảnh thumbnail cho video - chỉ dùng cho video, image = null
        /// </summary>
        public string PosterImage { get; set; }

        /// <summary>
        /// Chiều rộng ảnh/video (px) - dùng cho PhotoSwipe
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Chiều cao ảnh/video (px) - dùng cho PhotoSwipe
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
