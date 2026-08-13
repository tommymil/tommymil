const BREAK_SECONDS = 5 * 60;

// Samodzielny dokument okna przerwy: własne style i odliczanie w czystym JS,
// żeby okno działało niezależnie od aplikacji (można je przenieść na drugi ekran/rzutnik).
const breakDocument = `<!doctype html>
<html lang="pl">
<head>
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Przerwa</title>
<style>
  * { box-sizing: border-box; margin: 0; }
  html, body { height: 100%; }
  body {
    background: #0b1220;
    color: #f3f5f9;
    font-family: system-ui, -apple-system, "Segoe UI", Roboto, sans-serif;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 28px;
    text-align: center;
    padding: 24px;
  }
  .eyebrow {
    text-transform: uppercase;
    letter-spacing: 0.18em;
    font-weight: 700;
    color: #9aa7bd;
    font-size: clamp(14px, 3vw, 24px);
  }
  #timer {
    font-size: clamp(96px, 30vw, 360px);
    font-weight: 800;
    line-height: 1;
    font-variant-numeric: tabular-nums;
  }
  #timer.done { color: #4ade80; }
  #hint {
    color: #9aa7bd;
    font-size: clamp(16px, 3.4vw, 30px);
    font-weight: 600;
  }
  .actions { display: flex; flex-wrap: wrap; gap: 12px; justify-content: center; }
  button {
    background: #1b2333;
    color: #f3f5f9;
    border: 1px solid #2a3346;
    border-radius: 12px;
    padding: 12px 22px;
    font-size: clamp(15px, 2.4vw, 20px);
    font-weight: 700;
    cursor: pointer;
  }
  button:hover { background: #25304a; }
  button.primary { background: #2563eb; border-color: #2563eb; }
</style>
</head>
<body>
  <span class="eyebrow">Przerwa</span>
  <div id="timer">05:00</div>
  <div id="hint">Odpocznijcie, zaraz wracamy do pracy.</div>
  <div class="actions">
    <button id="toggle" type="button">Pauza</button>
    <button id="reset" type="button">Od nowa (5:00)</button>
    <button id="close" class="primary" type="button">Zamknij</button>
  </div>
  <script>
    (function () {
      var total = ${BREAK_SECONDS};
      var remaining = total;
      var running = true;
      var intervalId = null;
      var timerEl = document.getElementById("timer");
      var hintEl = document.getElementById("hint");
      var toggleEl = document.getElementById("toggle");

      function format(value) {
        var m = Math.floor(value / 60);
        var s = value % 60;
        return (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
      }

      function render() {
        timerEl.textContent = format(remaining);
        if (remaining === 0) {
          timerEl.className = "done";
          hintEl.textContent = "Koniec przerwy - wracamy do zajęć!";
          toggleEl.style.display = "none";
        }
      }

      function stop() {
        if (intervalId) { clearInterval(intervalId); intervalId = null; }
      }

      function start() {
        stop();
        intervalId = setInterval(function () {
          if (remaining > 0) { remaining -= 1; render(); }
          if (remaining === 0) { stop(); running = false; }
        }, 1000);
      }

      toggleEl.addEventListener("click", function () {
        running = !running;
        toggleEl.textContent = running ? "Pauza" : "Wznów";
        if (running) { start(); } else { stop(); }
      });

      document.getElementById("reset").addEventListener("click", function () {
        remaining = total;
        running = true;
        timerEl.className = "";
        hintEl.textContent = "Odpocznijcie, zaraz wracamy do pracy.";
        toggleEl.style.display = "";
        toggleEl.textContent = "Pauza";
        render();
        start();
      });

      document.getElementById("close").addEventListener("click", function () { window.close(); });
      document.addEventListener("keydown", function (event) { if (event.key === "Escape") { window.close(); } });

      render();
      start();
    })();
  </script>
</body>
</html>`;

/**
 * Otwiera odliczanie przerwy (5:00 -> 0:00) w osobnym oknie przeglądarki,
 * które można przenieść na drugi ekran/rzutnik. Zwraca null, gdy przeglądarka
 * zablokowała wyskakujące okno.
 */
export function openBreakWindow(): Window | null {
  const features = "width=640,height=480,menubar=no,toolbar=no,location=no,status=no";
  const popup = window.open("", "lesson-runner-break", features);

  if (!popup) {
    return null;
  }

  popup.document.open();
  popup.document.write(breakDocument);
  popup.document.close();
  popup.focus();

  return popup;
}
