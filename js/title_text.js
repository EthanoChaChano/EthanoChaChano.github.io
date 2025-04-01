async function init() {
    const node = document.querySelector("#type-text");
  
    await sleep(1000);
    node.innerText = "";
    await node.type("");
  
    while (true) {
        //Game Developer -> Game Designer -> Software Engineer -> Web Developer
      await node.type("Game Developer");
      await sleep(2000);

      await node.delete("Game Developer");
      await node.type("Game Designer");
      await sleep(2000);

      await node.delete("Game Designer");
      await node.type("Software Engineer");
      await sleep(2000);

      await node.delete("Software Engineer");
      await node.type("Web Developer");
      await sleep(2000);

      await node.delete("Web Developer");
    }
  }
  
  // Source code 🚩
  
  const sleep = (time) => new Promise((resolve) => setTimeout(resolve, time));
  
  class TypeAsync extends HTMLSpanElement {
    get typeInterval() {
      const randomMs = 100 * Math.random();
      return randomMs < 50 ? 10 : randomMs;
    }
  
    async type(text) {
      for (let character of text) {
        this.innerText += character;
        await sleep(this.typeInterval);
      }
    }
  
    async delete(text) {
      for (let character of text) {
        this.innerText = this.innerText.slice(0, this.innerText.length - 1);
        await sleep(this.typeInterval);
      }
    }
  }
  
  customElements.define("type-async", TypeAsync, { extends: "span" });
  
  init();
  