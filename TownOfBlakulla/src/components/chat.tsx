import * as React from "react";
import GameState from "../game-state";
import BlakullaService, { Subscription, ChatMessage, GameStateType } from "../blakulla-service";
import "./chat.scss";

export interface ChatProps {
    service: BlakullaService;
    messages: ChatMessage[];
    name: string;
    channel: string;
    enabled: boolean;
}

export interface GameChatState {
    message: string;
}

export default class Chat extends React.Component<ChatProps, GameChatState> {
    private readonly service: BlakullaService;
    private maxChatMessageCount:number = 100;

    constructor(props:ChatProps) {
        super(props);

        this.state = { message: "" };

        this.service = props.service;
        this.onMessageChanged = this.onMessageChanged.bind(this);
        this.sendMessageAsync = this.sendMessageAsync.bind(this);
        this.onInputKeyDown = this.onInputKeyDown.bind(this);
    }

    componentDidMount(): void {
    }

    componentWillUnmount(): void {
    }

    componentWillReceiveProps(props: ChatProps) {
        this.forceUpdate();
    }

    render() {
        if (!this.props.channel) {
            return (<div>Chat unavailable</div>);
        }

        const msgs = [...this.props.messages];
        msgs.reverse();

        const chatMessages = msgs.map(x =>
            (<div className={`chat-log-row${x.sender == this.props.name ? ' my-player' : ''}`} key={`${x.sender}-${x.timeSent}`}>
                &lt;{x.sender}&gt; {x.message}
            </div>));

        const placeholder = `Send a message to ${this.props.channel}`;

        return (

        <div className="chat-panel-input">
            <div className="input-row">
                <input placeholder={placeholder} value={this.state.message} onChange={this.onMessageChanged} onKeyDown={this.onInputKeyDown} />
                <button onClick={this.sendMessageAsync}>Send</button>
            </div>        
            <div className="chat-log">
                {chatMessages}
            </div>
        </div>);
    }

    onMessageChanged(e:React.ChangeEvent<HTMLInputElement>) {
        const value = e.currentTarget.value;        
        this.setState({
           message: value 
        })
    }

    onInputKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {        
        if (e.keyCode == 13) { // if enter
            e.preventDefault();
            this.sendMessageAsync();
        }
    }

    async sendMessageAsync() {
        const msg = this.state.message;      

        const channel = this.props.channel;
        if (!msg || msg == null || msg.length == 0 || msg.trim() == "") {
            return;
        }
        
        if (!channel) {
            console.warn("User trying to send to empty channel!");
            return;
        }

        this.setState({ message: "" });

        console.log(`Send message, #${channel}: ${msg}`);
        await this.service.sendChatMessageAsync(channel, msg);
    }
}
