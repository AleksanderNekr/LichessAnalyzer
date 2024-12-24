import os
from concurrent import futures
import grpc
import ApiGrpc.service_pb2 as service_pb2
import ApiGrpc.service_pb2_grpc as service_pb2_grpc
import joblib
import pandas as pd
from datetime import datetime, timedelta

LISTEN_PORT = os.environ.get('LISTEN_PORT', '50051')


class RatingPredictorServicer(service_pb2_grpc.RatingPredictorServicer):
    def __init__(self):
        # self.model = joblib.load('rating_predictor_model.pkl')
        pass

    def PredictRating(self, request, context):
        # Convert gRPC request to DataFrame
        ratings = [{'ds': datetime.strptime(rating.date, '%Y-%m-%d'), 'y': rating.rating} for
                   rating in request.ratings]
        df = pd.DataFrame(ratings)

        # Predict future ratings
        # future = self.model.make_future_dataframe(periods=request.num_predictions)
        # forecast = self.model.predict(future)
        forecast = pd.DataFrame({
            'ds': [datetime.now() + timedelta(days=i) for i in range(request.num_predictions)],
            'yhat': [1000 + i * 10 for i in range(request.num_predictions)],
        })

        # Prepare the response
        predictions = []
        for i in range(request.num_predictions):
            prediction = service_pb2.Rating(
                date=forecast.iloc[-(request.num_predictions - i)]['ds'].strftime('%Y-%m-%d'),
                rating=forecast.iloc[-(request.num_predictions - i)]['yhat']
            )
            predictions.append(prediction)

        return service_pb2.PredictRatingResponse(predictions=predictions)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    service_pb2_grpc.add_RatingPredictorServicer_to_server(RatingPredictorServicer(), server)
    server.add_insecure_port('[::]:%s' % LISTEN_PORT)
    server.start()
    server.wait_for_termination()


if __name__ == '__main__':
    print('Starting server. Listening on port %s.' % LISTEN_PORT)
    try:
        serve()
    except Exception as e:
        print('Error: %s' % e)
        raise
