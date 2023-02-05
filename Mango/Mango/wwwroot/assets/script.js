
const slideContainer = document.querySelectorAll('.slider');


for (let i = 0; i < slideContainer.length; i++) {

  const slider = function () {
  
    const slides = slideContainer[i].querySelectorAll('.slide');
    const btnLeft = slideContainer[i].querySelector('.slider__btn--left');
    const btnRight = slideContainer[i].querySelector('.slider__btn--right');
    const dotContainer = slideContainer[i].querySelector('.dots');

    let curSlide = 0;
    const maxSlide = slides.length;

    // Functions
    const createDots = function () {
      slides.forEach(function (_, i) {
        dotContainer.insertAdjacentHTML(
          'beforeend',
          `<button class="dots__dot" data-slide="${i}"></button>`
        );
      });
    };

    const activateDot = function (slide) {
      slideContainer[i]
        .querySelectorAll('.dots__dot')
        .forEach(dot => dot.classList.remove('dots__dot--active'));

      slideContainer[i]
        .querySelector(`.dots__dot[data-slide="${slide}"]`)
        .classList.add('dots__dot--active');
    };

    const goToSlide = function (slide) {
      slides.forEach(
        (s, i) => (s.style.transform = `translateX(${100 * (i - slide)}%)`)
      );
    };

    // Next slide
    const nextSlide = function () {
      if (curSlide === maxSlide - 1) {
        curSlide = 0;
      } else {
        curSlide++;
      }

      goToSlide(curSlide);
      activateDot(curSlide);
    };

    const prevSlide = function () {
      if (curSlide === 0) {
        curSlide = maxSlide - 1;
      } else {
        curSlide--;
      }
      goToSlide(curSlide);
      activateDot(curSlide);
    };

    const init = function () {
      goToSlide(0);
      createDots();

      activateDot(0);
    };
    init();

    // Event handlers
    btnRight.addEventListener('click', nextSlide);
    btnLeft.addEventListener('click', prevSlide);

    document.addEventListener('keydown', function (e) {
      if (e.key === 'ArrowLeft') prevSlide();
      e.key === 'ArrowRight' && nextSlide();
    });

    dotContainer.addEventListener('click', function (e) {
      if (e.target.classList.contains('dots__dot')) {
        const { slide } = e.target.dataset;
        goToSlide(slide);
        activateDot(slide);
      }
    });
  };
  slider();
}




var button = document.getElementById('mainButton');

var openForm = function () {
    button.className = 'active';
};

var checkInput = function (input) {
    if (input.value.length > 0) {
        input.className = 'active';
    } else {
        input.className = '';
    }
};

var closeForm = function () {
    button.className = '';
};

document.addEventListener("keyup", function (e) {
    if (e.keyCode == 27 || e.keyCode == 13) {
        closeForm();
    }
});









//jQuery(document).ready(function ($) {
//    var skipRow = 1;
//    $(document).on('click', '#loadMore', function () {
//        console.log("here")
//        $.ajax({
//            method: 'GET',
//            url: "/women/loadmore",
//            data: {
//                skipRow: skipRow
//            },
//            success: function (result) {
//                $('#mangogirlss').append(result);
//                skipRow++;
//            }
//        })
//    })
//});


let addToBasketBtns = document.querySelectorAll(".add-to-basket");


addToBasketBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();

    let url = btn.getAttribute("href");
    let counterSpan = document.querySelector(".basket-count")
    fetch(url)
        .then(response => {
            if (response.status == 200) {
                fetch("https://localhost:7119/product/GetBasketItems")
                    .then(response => response.json())
                    .then(data => {
                        counterSpan.innerHTML = data.length
                    })
           
            }
            else if (response.status == 401) {
                window.location.href= "https://localhost:7119/account/login"
            }
            else {
                alert("Error")
                window.location.reload(true)
            }
        })
}))

let addToFavouriteBtns = document.querySelectorAll(".add-to-favourites");


addToFavouriteBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();

    let url = btn.getAttribute("href");
    let counterSpan = document.querySelector(".favourite-item-count")
    fetch(url)
        .then(response => {
            if (response.status == 200) {
                fetch("https://localhost:7119/product/GetFavouriteItems")
                    .then(response => response.json())
                    .then(data => {
                        counterSpan.innerHTML = data.length
                    })
            } else {
                alert("Error")
                window.location.reload(true)
            }
        })
}))

let addToFavouriteWishlistPageBtns = document.querySelectorAll(".add-to-favourites-wishlist");


addToFavouriteWishlistPageBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();

    let url = btn.getAttribute("href");
    let counterSpan = document.querySelector(".favourite-item-count")
    fetch(url)
        .then(response => {
            if (response.status == 200) {
                fetch("https://localhost:7119/product/GetFavouriteItems")
                    .then(response => response.json())
                    .then(data => {
                        counterSpan.innerHTML = data.length
                        window.location.reload(true)
                    })
            } else {
                alert("Error")
                window.location.reload(true)
            }
        })
}))


let removeFromFavouritesBtns = document.querySelectorAll(".remove-from-basket");
removeFromFavouritesBtns.forEach(btn => btn.addEventListener("click", function(e) {
    e.preventDefault();
    let url = btn.getAttribute("href")
    fetch(url)
        .then(response => {
            if (response.status == 200) {
                window.location.reload(true);
            }
            else {
                alert("Error")
                window.location.reload(true)
            }

        })
}))


let heartIcons = document.querySelectorAll(".heart-icon");

heartIcons.forEach(icon => icon.addEventListener("click", function () {
    if (icon.classList.contains("fa-regular")) {
        icon.classList.remove("fa-regular")
        icon.classList.add("fa-solid")
    } else {
        icon.classList.remove("fa-solid")
        icon.classList.add("fa-regular")
    }

}))








