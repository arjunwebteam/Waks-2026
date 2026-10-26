// countdown js 

const countdown = new Date(2026, 7, 31, 0, 0, 0);

    const daysCard = document.querySelector(".days .countdown-flip");
    const hoursCard = document.querySelector(".hours .countdown-flip");

    function getTimeRemaining(endtime) {
      const diff = endtime - new Date();

      return {
        diff,
        days: Math.floor(diff / (1000 * 60 * 60 * 24)),
        hours: Math.floor((diff / (1000 * 60 * 60)) % 24)
      };
    }

    function addFlip(card, time) {
      const top = card.querySelector(".countdown-top");
      const bottom = card.querySelector(".countdown-bottom");

      const current = top.innerText;
      const next = time < 10 ? `0${time}` : `${time}`;

      if (current === next) return;

      const topFlip = document.createElement("div");
      const bottomFlip = document.createElement("div");

      topFlip.className = "countdown-top-flip";
      bottomFlip.className = "countdown-bottom-flip";

      topFlip.innerText = current;

      topFlip.addEventListener("animationstart", () => top.innerText = next);
      topFlip.addEventListener("animationend", () => {
        topFlip.remove();
        bottomFlip.innerText = next;
      });

      bottomFlip.addEventListener("animationend", () => {
        bottom.innerText = next;
        bottomFlip.remove();
      });

      card.append(topFlip, bottomFlip);
    }

    setInterval(() => {
      const t = getTimeRemaining(countdown);
      addFlip(daysCard, t.days);
      addFlip(hoursCard, t.hours);
    }, 1000);

    // countdown js end