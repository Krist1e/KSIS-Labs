class Slider {
  constructor(containerSelector, prevBtnSelector, nextBtnSelector) {
      this.container = document.querySelector(containerSelector);
      if (!this.container) return;
      this.prevBtn = document.querySelector(prevBtnSelector);
      this.nextBtn = document.querySelector(nextBtnSelector);
      this.slider = this.container.querySelector('.slider');

      this.sliderItems = this.slider.querySelectorAll('.slider__item');
      this.currentScroll = 0;
      this.slideWidth = this.sliderItems[0].clientWidth;
      this.containerWidth = this.container.clientWidth;
      this.currentStepWidth = Math.floor(this.containerWidth / this.slideWidth);

      this.prevBtn.addEventListener('click', this.prevSlide.bind(this));
      this.nextBtn.addEventListener('click', this.nextSlide.bind(this));

      window.addEventListener('resize', this.updateStepWidth.bind(this));
      window.addEventListener('load', this.updateStepWidth.bind(this));

      this.slider.addEventListener('scroll', this.updateCurrentScroll.bind(this));

      this.slider.addEventListener('touchstart', (e) => {
          this.startX = e.touches[0].clientX;
      });

      this.slider.addEventListener('touchend', (e) => {
          this.endX = e.changedTouches[0].clientX;

          if (this.endX < this.startX) {
              this.nextSlide();
          } else {
              this.prevSlide();
          }
      });
  }

  updateStepWidth() {
      this.containerWidth = this.container.clientWidth;
      this.currentStepWidth = Math.floor(this.containerWidth / this.slideWidth);
  }

  updateCurrentScroll() {
      this.currentScroll = this.slider.scrollLeft;
  }

  prevSlide() {
      if (this.currentScroll === 0) {
          this.currentScroll = -this.slideWidth * (this.sliderItems.length - this.currentStepWidth);
      } else if (this.currentScroll < 0) {
          this.currentScroll = 0;
      }

      this.slider.scrollBy({
          left: -this.slideWidth * this.currentStepWidth,
          behavior: 'smooth'
      });
  }

  nextSlide() {
      if (this.currentScroll < 0) {
          this.currentScroll = 0;
      } else if (this.currentScroll > 0) {
          this.currentScroll = -this.slideWidth * (this.sliderItems.length - this.currentStepWidth);
      }

      this.slider.scrollBy({
          left: this.slideWidth * this.currentStepWidth,
          behavior: 'smooth'
      });
  }
}

function useDynamicAdapt(type = 'max') {
  const className = '_dynamic_adapt_'
  const attrName = 'data-da'

  const dNodes = getDNodes()

  const dMediaQueries = getDMediaQueries(dNodes)

  dMediaQueries.forEach((dMediaQuery) => {
      const matchMedia = window.matchMedia(dMediaQuery.query)
      const filteredDNodes = dNodes.filter(({ breakpoint }) => breakpoint === dMediaQuery.breakpoint)
      const mediaHandler = getMediaHandler(matchMedia, filteredDNodes)
      matchMedia.addEventListener('change', mediaHandler)

      mediaHandler()
  })

  function getDNodes() {
      const result = []
      const elements = [...document.querySelectorAll(`[${attrName}]`)]

      elements.forEach((element) => {
          const attr = element.getAttribute(attrName)
          const [toSelector, breakpoint, order] = attr.split(',').map((val) => val.trim())

          const to = document.querySelector(toSelector)

          if (to) {
              result.push({
                  parent: element.parentElement,
                  element,
                  to,
                  breakpoint: breakpoint ?? '767',
                  order: order !== undefined ? (isNumber(order) ? Number(order) : order) : 'last',
                  index: -1,
              })
          }
      })

      return sortDNodes(result)
  }

  function getDMediaQueries(items) {
      const uniqItems = [...new Set(items.map(({ breakpoint }) => `(${type}-width: ${breakpoint}px),${breakpoint}`))]

      return uniqItems.map((item) => {
          const [query, breakpoint] = item.split(',')

          return { query, breakpoint }
      })
  }

  function getMediaHandler(matchMedia, items) {
      return function mediaHandler() {
          if (matchMedia.matches) {
              items.forEach((item) => {
                  moveTo(item)
              })

              items.reverse()
          } else {
              items.forEach((item) => {
                  if (item.element.classList.contains(className)) {
                      moveBack(item)
                  }
              })

              items.reverse()
          }
      }
  }

  function moveTo(dNode) {
      const { to, element, order } = dNode
      dNode.index = getIndexInParent(dNode.element, dNode.element.parentElement)
      element.classList.add(className)

      if (order === 'last' || order >= to.children.length) {
          to.append(element)

          return
      }

      if (order === 'first') {
          to.prepend(element)

          return
      }

      to.children[order].before(element)
  }

  function moveBack(dNode) {
      const { parent, element, index } = dNode
      element.classList.remove(className)

      if (index >= 0 && parent.children[index]) {
          parent.children[index].before(element)
      } else {
          parent.append(element)
      }
  }

  function getIndexInParent(element, parent) {
      return [...parent.children].indexOf(element)
  }

  function sortDNodes(items) {
      const isMin = type === 'min' ? 1 : 0

      return [...items].sort((a, b) => {
          if (a.breakpoint === b.breakpoint) {
              if (a.order === b.order) {
                  return 0
              }

              if (a.order === 'first' || b.order === 'last') {
                  return -1 * isMin
              }

              if (a.order === 'last' || b.order === 'first') {
                  return 1 * isMin
              }

              return 0
          }

          return (a.breakpoint - b.breakpoint) * isMin
      })
  }

  function isNumber(value) {
      return !isNaN(value)
  }
}

const iconMenu = document.querySelector('.icon-menu');
if (iconMenu) {
  const menuBody = document.querySelector('.menu-body');
  iconMenu.addEventListener("click", function (e) {
    document.body.classList.toggle('lock');
    iconMenu.classList.toggle('active');
    menuBody.classList.toggle('active');
  });
}

const filterButton = document.querySelector('.button-filter');
if (filterButton) {
  const filterBody = document.querySelector('.filter');
  filterButton.addEventListener("click", function (e) {
    document.body.classList.toggle('lock');
    filterButton.classList.toggle('active');
    filterBody.classList.toggle('active');
  });
}

useDynamicAdapt();

new Slider('.featured__container', '.featured__prev', '.featured__next');
new Slider('.bestsellers__container', '.bestsellers__prev', '.bestsellers__next');