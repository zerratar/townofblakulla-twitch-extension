"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
const ReactDOM = require("react-dom");
const hello_1 = require("./components/hello");
const video_component_1 = require("./components/video_component");
let target = document.getElementById("video_component")
    || document.getElementById("example");
if (target.id == "video_component") {
    ReactDOM.render(React.createElement(video_component_1.VideoComponent, { compiler: "TypeScript", framework: "React" }), target);
}
else {
    ReactDOM.render(React.createElement(hello_1.Hello, { compiler: "TypeScript", framework: "React" }), target);
}
//# sourceMappingURL=index.js.map