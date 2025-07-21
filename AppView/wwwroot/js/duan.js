// let slideIndex = 0;
// showSlides();

// function showSlides() {
//   let slides = document.getElementsByClassName("slide");

//   // Ẩn tất cả các slide
//   for (let i = 0; i < slides.length; i++) {
//     slides[i].style.display = "none";
//   }

//   // Tăng chỉ số slide và quay về 0 nếu vượt quá số lượng ảnh
//   slideIndex++;
//   if (slideIndex > slides.length) {
//     slideIndex = 1;
//   }

//   // Hiển thị slide hiện tại
//   slides[slideIndex - 1].style.display = "block";

//   // Thêm hiệu ứng fade
//   slides[slideIndex - 1].classList.add("fade");

//   // Chuyển ảnh sau 3 giây (3000ms)
//   setTimeout(showSlides, 2000);
// }
// function plusSlides(n) {
//     slideIndex += n;
//     let slides = document.getElementsByClassName("slide");
//     if (slideIndex > slides.length) { slideIndex = 1; }
//     if (slideIndex < 1) { slideIndex = slides.length; }
//     for (let i = 0; i < slides.length; i++) {
//       slides[i].style.display = "none";
//     }
//     slides[slideIndex - 1].style.display = "block";
//   }
//   function currentSlide(n) {
//     slideIndex = n;
//     showSlides();
//   }
  // danh mục
  
  
  
  // Bắt sự kiện click cho từng bộ lọc
  document.querySelectorAll('.filter-item').forEach(item => {
    item.addEventListener('click', function (e) {
      e.stopPropagation();
      // Ẩn các popup khác
      document.querySelectorAll('.popup').forEach(p => p.style.display = 'none');

      const popup = this.querySelector('.popup');
      if (popup) {
        popup.style.display = 'block';
      }
    });
  });

  // Click ngoài để ẩn tất cả popup
  document.addEventListener('click', () => {
    document.querySelectorAll('.popup').forEach(p => p.style.display = 'none');
  });

  // Xử lý chọn giá trị (tùy bạn xử lý thêm)
  document.querySelectorAll('.size-option, .color-circle, .price-option').forEach(option => {
    option.addEventListener('click', (e) => {
      const value = e.target.innerText || e.target.dataset.color;
      console.log("Đã chọn:", value);
    });
  });


  // document.querySelectorAll('.filter-item').forEach(item => {
  //     const popup = item.querySelector('.popup');

  //     item.querySelector('.filter-label').addEventListener('click', function (e) {
  //       e.stopPropagation();
  //       togglePopup(popup);
  //     });

  //     item.querySelector('.arrow').addEventListener('click', function (e) {
  //       e.stopPropagation();
  //       togglePopup(popup);
  //     });
  //   });

  //   // Ẩn popup khi click bên ngoài
  //   document.addEventListener('click', () => {
  //     document.querySelectorAll('.popup').forEach(p => p.style.display = 'none');
  //   });

  //   // Toggle popup
  //   function togglePopup(popup) {
  //     const isOpen = popup.style.display === 'block';
  //     document.querySelectorAll('.popup').forEach(p => p.style.display = 'none');
  //     popup.style.display = isOpen ? 'none' : 'block';
  //   }

  //   // Xử lý chọn giá trị
  //   document.querySelectorAll('.size-option, .color-circle, .price-option').forEach(option => {
  //     option.addEventListener('click', (e) => {
  //       const value = e.target.innerText || e.target.dataset.color;
  //       console.log("Đã chọn:", value);
  //     });
  //   });


// kết thúc danh mục
