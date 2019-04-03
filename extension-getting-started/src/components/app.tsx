import * as React from "react";

import Authentication from "../authentication";
import TwitchComponentState from "../twitch-component-state";

export interface AppProps { compiler: string; framework: string; }

export class App extends React.Component<AppProps, TwitchComponentState> {
    private auth: Authentication;
    private twitch: any;
    constructor(props: AppProps) {
        super(props);
        let win = window as any;
        this.auth = new Authentication(null, null);
        this.twitch = win.Twitch ? win.Twitch.ext : null;
        this.state = new TwitchComponentState();
    }
    render() {
        if (this.state.finishedLoading && this.state.isVisible) {
            return (
                <div className="App">
                    <div className={this.state.theme === 'light' ? 'App-light' : 'App-dark'} >
                        <p>{JSON.stringify(this.state.test)}</p>
                        <p>{this.state.username}</p>
                        <p>Hello world!</p>
                        <p>My token is: {this.auth.state.token}</p>
                        <p>My opaque ID is {this.auth.getOpaqueId()}.</p>
                        <div>{this.auth.isModerator() ? <p>I am currently a mod, and here's a special mod button <input value='mod button' type='button' /></p> : 'I am currently not a mod.'}</div>
                        <p>I have {this.auth.hasSharedId() ? `shared my ID, and my user_id is ${this.auth.getUserId()}` : 'not shared my ID'}.</p>
                    </div>
                </div>
            )
        } else {
            return (
                <div className="App">
                    Loading...
                </div>
            )
        }
    }

    contextUpdate(context: any, delta: any) {
        if (delta.includes('theme')) {
            this.setState(() => {
                return { theme: context.theme }
            })
        }
    }

    visibilityChanged(isVisible: boolean) {
        this.setState(() => {
            return {
                isVisible
            }
        })
    }

    componentDidMount() {
        if (this.twitch) {
            this.twitch.onAuthorized((auth: any) => {
                this.auth.setToken(auth.token, auth.userId)

                console.log("onAuthorized!");

                if (!this.state.finishedLoading) {

                    this.twitch.actions.requestIdShare((e: any) => { 
                        console.log(e)

                        this.auth.apiGet("test").then(result => {
                            this.setState(() => {
                                return { test: result, username: e }
                            })
                        })
                     });


                    // if the component hasn't finished loading (as in we've not set up after getting a token), let's set it up now.
                    // now we've done the setup for the component, let's set the state to true to force a rerender with the correct data.
                    this.setState(() => {
                        return { finishedLoading: true }
                    })
                }
            })

            this.twitch.listen('broadcast', (target: any, contentType: any, body: any) => {
                this.twitch.rig.log(`New PubSub message!\n${target}\n${contentType}\n${body}`)
                // now that you've got a listener, do something with the result... 

                // do something...

            })

            this.twitch.onVisibilityChanged((isVisible: boolean, _c: any) => {
                this.visibilityChanged(isVisible)
            })

            this.twitch.onContext((context: any, delta: any) => {
                this.contextUpdate(context, delta)
            })
        }
    }

    componentWillUnmount() {
        if (this.twitch) {
            this.twitch.unlisten('broadcast', () => console.log('successfully unlistened'))
        }
    }
}