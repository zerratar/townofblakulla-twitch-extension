import * as React from "react";
import GameState from "../../game-state";
import BlakullaService from "../../blakulla-service";
import { PhaseProps } from "./phase-props";
import Chat from "../chat";

export default class Day extends React.Component<PhaseProps, GameState> {
    private readonly service: BlakullaService;
    constructor(props:PhaseProps) {
        super(props);
        this.state = props.gameState;
        this.service = props.service;
        this.voteForLynch = this.voteForLynch.bind(this);
        this.voteGuilty = this.voteGuilty.bind(this);
        this.voteInnocent = this.voteInnocent.bind(this);
    }

    componentWillReceiveProps(props: any) {
        
        this.setState(()=> { return { ...props.gameState }});
    }

    render() {

        let subPhaseRender = null;
        if (!this.state.lynched) {
            switch (this.state.game.subPhase) {
                case "Voting":
                    subPhaseRender = this.renderVoting();
                    break;
                case "Judgement":
                    subPhaseRender = this.renderJudgement();
                    break;
            }
        }
        const channelName = this.props.channel;
        const isEnabled = !this.state.lynched;
        return (

            <div>
                <div>Day - {this.state.game.subPhase} [{this.state.role}]</div>
                <div className="sub-phase">
                    {subPhaseRender}
                </div>
                <Chat 
                    name={this.state.name}
                    messages={this.state.chatMessages}
                    channel={channelName}
                    enabled={isEnabled}
                    service={this.service}>
                </Chat>
            </div>
        );
    }

    renderJudgement(): any {
        return (<div>Judgement is upon us!
            <button onClick={this.voteGuilty}>Guilty</button>
            <button onClick={this.voteInnocent}>Innocent</button>
        </div>);
    }

    renderVoting(): any {
        if (!this.state.game.players) {
            return (<p></p>);
        }

        const players = this.state.game.players;
        return (<div>
                Who should we lynch?
                {players.filter(x => x.name != this.state.name).map(x => (
                    <button key={x.name} onClick={this.voteForLynch} data-name={x.name} disabled={x.lynched}>{x.name}</button>
                ))}
            </div>);
    }

    async voteGuilty() {
        console.log(`Vote guilty!`);
        const result = await this.service.voteAsync("guilty");
        console.log(JSON.stringify(result));
    }

    async voteInnocent() {
        console.log(`Vote innocent!`);
        const result = await this.service.voteAsync("innocent");
        console.log(JSON.stringify(result));
    }

    async voteForLynch(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {        
        const btn = event.currentTarget as HTMLButtonElement;
        const playerName = btn.dataset.name; 
        console.log(`Lynch ${playerName}, eh?`)
        const result = await this.service.voteAsync(playerName);
        console.log(JSON.stringify(result));
    }
}