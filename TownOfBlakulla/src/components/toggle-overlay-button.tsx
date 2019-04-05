import * as React from "react";
import "./toggle-overlay-button.scss";


export interface ToggleOverlayButtonProps {
    onVisibilityChanged: (isVisible: boolean) => void;
}
export class ToggleOverlayButtonState {
    public isVisible: boolean = false;
}

export class ToggleOverlayButton extends React.Component<ToggleOverlayButtonProps, ToggleOverlayButtonState> {
    constructor(props: any) {
        super(props);
        this.state = new ToggleOverlayButtonState();
        this.toggleOverlayVisibility = this.toggleOverlayVisibility.bind(this);
    }
    
    render() {
        if (this.state.isVisible) {
            return (<div className="toggle-overlay-panel">
                <button onClick={this.toggleOverlayVisibility}>&times;</button>
            </div>);
        }
        return (<div className="toggle-overlay-panel">
            <button onClick={this.toggleOverlayVisibility}>&#9658;</button>
        </div>);
    }

    toggleOverlayVisibility(): void {
        const visibility = !this.state.isVisible;
        if (this.props.onVisibilityChanged)
            this.props.onVisibilityChanged(visibility);
        this.setState(() => {
            return { isVisible: visibility };
        });
    }
}
