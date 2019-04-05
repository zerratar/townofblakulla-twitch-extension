"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
// export const Hello = (props: HelloProps) => <h1>Hello from {props.compiler} and {props.framework}!</h1>;
class Hello extends React.Component {
    render() {
        return React.createElement("h1", null,
            "Hello from ",
            this.props.compiler,
            " and ",
            this.props.framework,
            "!");
    }
}
exports.Hello = Hello;
//# sourceMappingURL=hello.js.map