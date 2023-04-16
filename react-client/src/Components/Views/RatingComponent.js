import { Component } from 'react';
import { useState } from 'react';
import axios from 'axios';


export class RatingComponent extends Component {

    clearRatings = () => {
        this.ratingsArr = [false, false, false, false, false];
    }

    deleteRating = (index) => {
        var ratingComp = this;
        axios({
            url: 'https://localhost:44332/ratings',
            method: 'delete',
            data: {
                ListingId: this.props.listing.id,
            }
        }).then((response) => {
            if (response.status == 200) {
                ratingComp.deleteRatingSuccess(index);
            }
        });
    }

    deleteRatingSuccess = (index) => {
        this.listing = this.props.listing;
        if (this.listing.ratingCount == 1) {
            this.listing.rating = 0;
            this.listing.ratingCount = 0;
        }
        else {
            this.listing.rating = (this.listing.rating * this.listing.ratingCount - index - 1) / (this.listing.ratingCount - 1);
            this.listing.ratingCount--;
        }
        this.listing.userHasRated = false;

        this.setState({ listing: this.listing });

        this.clearRatings();
        this.setState({ ratingsArr: this.ratingsArr })
    }


    postRating = (index) => {
        var ratingComp = this;
        axios({
            url: 'https://localhost:44332/ratings',
            method: 'post',
            data: {
                ListingId: this.props.listing.id,
                Rating: index + 1
            }
        }).then((response) => {
            if (response.status == 200) {
                ratingComp.postRatingSuccess(index);
            }
        });
    }

    postRatingSuccess = (index) => {
        this.listing = this.props.listing;
        if (this.listing.userHasRated) {
            this.listing.rating = (this.listing.rating * this.listing.ratingCount - this.listing.userRating);
            this.listing.userRating = index + 1;
            this.listing.rating += this.listing.userRating;
            this.listing.rating /= this.listing.ratingCount;
        }
        else {
            this.listing.userRating = index + 1;
            this.listing.rating = (this.listing.rating * this.listing.ratingCount + this.listing.userRating) / (this.listing.ratingCount + 1);
            this.listing.ratingCount++;
            this.listing.userHasRated = true;
        }
        this.setState({ listing: this.listing });

        this.clearRatings();
        this.ratingsArr[index] = true;
        this.setState({ ratingsArr: this.ratingsArr });
    }

    ratingChanged = (e) => {
        //check if radio button is checked
        if (this.ratingsArr[parseInt(e.target.value)]) {
            this.deleteRating(parseInt(e.target.value));
        }
        else {
            this.postRating(parseInt(e.target.value));
        }
    }



    constructor(props) {
        super(props);

        this.ratingsArr = [false, false, false, false, false];
        this.rating = this.props.listing.rating;

        this.state = {
            ratingsArr: this.ratingsArr,
            rating: this.props.listing.rating
        }

        if (props.listing.userHasRated) {

            this.ratingsArr[parseInt(props.listing.userRating) - 1] = true;
            this.setState({ ratingsArr: this.ratingsArr });
        }
    }

    render() {
        var randomName = "ratingStar" + Math.random();
        return (
            <div className="rating">
                <div className="ratingStars">
                    {this.ratingsArr.map((isChecked, i) => {
                        return (
                            <input type="radio" className="ratingStar" name={randomName} onClick={this.ratingChanged} value={4 - i}
                                checked={this.ratingsArr[4 - i]}>
                            </input>)
                    })}
                </div>
                <div className="serverRating" style={{ width: this.props.listing.rating * 20 + "px" }}>
                </div>
                <p className="ratingText">Rating count: {this.props.listing.ratingCount} Average rating: {this.props.listing.rating}</p>
            </div >
        );
    }
}