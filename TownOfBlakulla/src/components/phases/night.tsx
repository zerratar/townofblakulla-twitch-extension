import * as React from "react";
import GameState from "../../game-state";
import { PhaseProps } from "./phase-props";
import BlakullaService from "../../blakulla-service";
import Chat from "../chat";

export default class Night extends React.Component<PhaseProps, GameState> {
    private readonly service: BlakullaService;
    constructor(props:PhaseProps) {
        super(props);
        this.state = props.gameState;
        this.service = props.service;
    }

    componentWillReceiveProps(props: PhaseProps) {
        this.setState({ 
            ...props.gameState
        });
    }

    render() {
        const channelName = this.props.channel;
        const isEnabled = !this.state.lynched;
        return (<div>
                <div>Night - {this.state.game.subPhase} [{this.state.role}]</div>
                <div className="sub-phase">
                </div>
                <Chat 
                    name={this.state.name}
                    channel={channelName}
                    enabled={isEnabled}
                    messages={this.state.chatMessages}
                    service={this.service}>
                </Chat>
            </div>
        );
    }
}